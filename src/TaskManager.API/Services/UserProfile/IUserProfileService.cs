using System;
using BaseLibrary.DTOs.SettingsDtos;
using BaseLibrary.Responses;

namespace TaskManager.Services.UserProfile;

public interface IUserProfileService
{
    Task<UserProfileResponse> GetProfileAsync();
    Task ChangePasswordAsync(ChangePasswordRequest request);
    Task UpdateSettingsAsync(UpdateUserSettingsRequest request);
    Task DeleteAccountAsync();
}
