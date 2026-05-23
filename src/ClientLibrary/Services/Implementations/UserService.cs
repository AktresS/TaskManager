
using System.Net.Http.Json;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class UserService (GetHttpClient getHttpClient) : IUserService
{
    private const string BaseUrl = "api/users";

    public async Task<List<UserSearchResponse>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new();
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<UserSearchResponse>>($"{BaseUrl}/search?q={Uri.EscapeDataString(query)}")
            ?? new();
    }
}
