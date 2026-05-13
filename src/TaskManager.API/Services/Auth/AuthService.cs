
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Security;

namespace TaskManager.Services.Auth;

public class AuthService(IOptions<JwtOptions> config, AppDbContext context) : IAuthService
{
    public async Task<GeneralResponse> CreateAsync(Register user)
    {
        if (user is null) return new GeneralResponse(false, "Model is empty");

        var checkUser = await context.Users.FirstOrDefaultAsync(x => x.Email!.ToLower()!.Equals(user.Email!.ToLower()));
        if (checkUser != null) return new GeneralResponse(false, "User registered already");

        var applicationUser = await AddToDatabase(new User
        {
            Email = user.Email!,
            Name = user.Name!,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password)
        });

        return new GeneralResponse(true, "Account created!");
    }

    public async Task<LoginResponse> SignInAsync(Login user)
    {
        if (user is null) return new LoginResponse(false, "Model is empty");

        var applicationUser = await context.Users.FirstOrDefaultAsync(x => x.Email!.ToLower()!.Equals(user.Email!.ToLower()));
        if (applicationUser is null) return new LoginResponse(false, "User registered already");

        if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.PasswordHash))
            return new LoginResponse(false, "Email/Password not valid");

        var jwtToken = GenerateToken(applicationUser);
        var refreshToken = GenerateRefreshToken();

        var findUser = await context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == applicationUser.UserId);
        if (findUser is not null)
        {
            findUser!.Token = refreshToken;
            findUser.Expires = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
        }
        else
        {
            await AddToDatabase(new RefreshToken() { Token = refreshToken, UserId = applicationUser.UserId, Expires = DateTime.UtcNow.AddDays(7)});
        }
        return new LoginResponse(true, "Login successfully", jwtToken, refreshToken);
    }

    private string GenerateToken(User user)
    {
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.SecretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: config.Value.Issuer,
            audience: config.Value.Audience,
            claims: userClaims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private async Task<T> AddToDatabase<T>(T model)
    {
        var result = context.Add(model!);
        await context.SaveChangesAsync();
        return (T)result.Entity;
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenValue token)
    {
        if (token is null) return new LoginResponse(false, "Model is empty");

        var findToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token!.Equals(token.Token));
        if (findToken is null) return new LoginResponse(false, "Refresh token is required");
        if (findToken.Expires < DateTime.UtcNow) return new LoginResponse(false, "Token expired");

        var user = await context.Users.FirstOrDefaultAsync(x => x.UserId == findToken.UserId);
        if (user is null) return new LoginResponse(false, "Refresh token could not be generated because user not found");

        string jwtToken = GenerateToken(user);
        string refreshToken = GenerateRefreshToken();

        var updateRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == user.UserId);
        if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token could not be generated because user has not signed in");

        Console.WriteLine($"[DEBUG] findToken.Expires: {updateRefreshToken.Expires} (Kind: {updateRefreshToken.Expires.Kind})");
        await context.SaveChangesAsync();
        return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
    }
}
