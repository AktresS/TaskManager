
using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.BoardDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class BoardService(GetHttpClient getHttpClient) : IBoardService
{
    private const string BaseUrl = "api/projects";
    public async Task<BoardBaseResponse> CreateAsync(int projectId, CreateBoardRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/{projectId}/board", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<BoardBaseResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<BoardBaseResponse>> GetByProjectAsync(int projectId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<BoardBaseResponse>>($"{BaseUrl}/{projectId}/board")
            ?? new();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{projectId}/board/{id}");

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }
    }
}
