using System;
using BaseLibrary.DTOs.SettingsDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IUserProfileService
{
    Task<UserProfileResponse> GetProfileAsync();
    Task ChangePasswordAsync(ChangePasswordRequest request);
    Task UpdateSettingsAsync(UpdateUserSettingsRequest request);
    Task DeleteAccountAsync();
}
