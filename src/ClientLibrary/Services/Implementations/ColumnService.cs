
using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.ColumnDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class ColumnService(GetHttpClient getHttpClient) : IColumnService
{
    private const string BaseUrl = "api/boards";
    public async Task<ColumnBaseResponse> CreateAsync(int boardId, CreateColumnRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/{boardId}/columns", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<ColumnBaseResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<ColumnBaseResponse>> GetByBoardAsync(int boardId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<ColumnBaseResponse>>($"{BaseUrl}/{boardId}/columns")
            ?? new();
    }

    public async Task<ColumnBaseResponse> UpdateAsync(int columnId, int boardId, UpdateColumnRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PatchAsJsonAsync($"{BaseUrl}/{boardId}/columns/{columnId}", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);
        
        return JsonSerializer.Deserialize<ColumnBaseResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task DeleteAsync(int columnId, int boardId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{boardId}/columns/{columnId}");

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }
    }
}
