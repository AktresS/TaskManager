
using System.Net.Http.Json;
using BaseLibrary.DTOs.SettingsDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class UserProfileService(GetHttpClient getHttpClient) : IUserProfileService
{
    private const string BaseUrl = "api/profile";

    public async Task<UserProfileResponse> GetProfileAsync()
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<UserProfileResponse>(BaseUrl)
            ?? throw new Exception("Failed to load profile");
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/change-password", request);

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    public async Task UpdateSettingsAsync(UpdateUserSettingsRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PutAsJsonAsync($"{BaseUrl}/settings", request);

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    public async Task DeleteAccountAsync()
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync(BaseUrl);

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }
}
