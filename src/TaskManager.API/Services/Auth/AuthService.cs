using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos.Auth;
using TaskManager.Models;
using TaskManager.Security;

namespace TaskManager.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(AppDbContext context, JwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(x => x.Email == request.Email))
            return null;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            AccessToken = token,
            UserId = user.UserId,
            Email = user.Email,
            Name = user.Name
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
            return null;

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!valid)
            return null;

        var accessToken = _tokenGenerator.GenerateToken(user);

        var refreshTokenValue = GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.UserId,
            Token = refreshTokenValue,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            UserId = user.UserId,
            Email = user.Email,
            Name = user.Name
        };
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (refreshToken == null)
            throw new Exception("Invalid refresh token");

        if (refreshToken.IsRevoked)
            throw new Exception("Token revoked");

        if (refreshToken.Expires < DateTime.UtcNow)
            throw new Exception("Token expired");

        refreshToken.IsRevoked = true;

        var newAccessToken = _tokenGenerator.GenerateToken(refreshToken.User);

        var newRefreshTokenValue = GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = newRefreshTokenValue,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(newRefreshToken);

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            UserId = refreshToken.User.UserId,
            Email = refreshToken.User.Email,
            Name = refreshToken.User.Name
        };
    }
}
