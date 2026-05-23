using System;
using BaseLibrary.DTOs.SettingsDtos;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;

namespace TaskManager.Services.UserProfile;

public class UserProfileService(AppDbContext context, ICurrentUserService currentUser) : IUserProfileService
{
    public async Task<UserProfileResponse> GetProfileAsync()
    {
        var user = await context.Users
            .Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.UserId == currentUser.UserId);

        if (user is null)
            throw new Exception("User not found");

        if (user.Settings is null)
        {
            var settings = new UserSettings { UserId = user.UserId };
            context.UserSettings.Add(settings);
            await context.SaveChangesAsync();
            user.Settings = settings;
        }

        return new UserProfileResponse
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            NotificationsEnabled = user.Settings.NotificationsEnabled,
            DeadlineNotificationsEnabled = user.Settings.DeadlineNotificationsEnabled,
            MessageNotificationsEnabled  = user.Settings.MessageNotificationsEnabled
        };
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await context.Users.FindAsync(currentUser.UserId);
        if (user is null)
            throw new Exception("User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new Exception("Неверный текущий пароль");

        if (request.NewPassword != request.ConfirmPassword)
            throw new Exception("Пароли не совпадают");

        if (request.NewPassword.Length < 6)
            throw new Exception("Пароль должен быть не менее 6 символов");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSettingsAsync(UpdateUserSettingsRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var user = await context.Users.FindAsync(currentUser.UserId);
            if (user is not null)
                user.Name = request.Name;
        }

        var settings = await context.UserSettings
            .FirstOrDefaultAsync(x => x.UserId == currentUser.UserId);

        if (settings is null)
        {
            settings = new UserSettings { UserId = currentUser.UserId };
            context.UserSettings.Add(settings);
        }

        settings.NotificationsEnabled         = request.NotificationsEnabled;
        settings.DeadlineNotificationsEnabled = request.DeadlineNotificationsEnabled;
        settings.MessageNotificationsEnabled  = request.MessageNotificationsEnabled;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync()
    {
        var user = await context.Users.FindAsync(currentUser.UserId);
        if (user is null)
            throw new Exception("User not found");

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}