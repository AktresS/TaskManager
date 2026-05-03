using System;
using System.Net.Http.Json;
using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
{
    public const string AuthUrl = "api/auth";
    public async Task<GeneralResponse> CreateAsync(Register user)
    {
        var httpClient = getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            var message = string.IsNullOrEmpty(error) ? (result.ReasonPhrase ?? "Server error") : error;
            return new GeneralResponse(false, message);
        }
        
        return await result.Content.ReadFromJsonAsync<GeneralResponse>() ?? new GeneralResponse(false, "Failed to parse server response");
    }

    public async Task<LoginResponse> SignInAsync(Login user)
    {
        var httpClient = getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            var message = string.IsNullOrEmpty(error) ? (result.ReasonPhrase ?? "Server error") : error;
            return new LoginResponse(false, message);
        }
        
        return await result.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse(false, "Failed to parse server response");
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenValue token)
    {
        var httpClient = getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            var message = string.IsNullOrEmpty(error) ? (result.ReasonPhrase ?? "Server error") : error;
            return new LoginResponse(false, message);
        }
        
        return await result.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse(false, "Failed to parse server response");
    }

    public async Task<WeatherForecast[]> GetWeatherForecastsAsync()
    {
        var httpClient = await getHttpClient.GetPrivateHttpClient();
        var result = await httpClient.GetFromJsonAsync<WeatherForecast[]>("api/weatherforecast");
        return result!;
    }
}
