
using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.TaskDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class WorkItemService(GetHttpClient getHttpClient) : IWorkItemService
{
    private const string BaseUrl = "api/workitems";
    public async Task<List<WorkItemResponse>> GetMyAsync()
    {
        var client = await getHttpClient.GetPrivateHttpClient();

        return await client.GetFromJsonAsync<List<WorkItemResponse>>($"{BaseUrl}/my-tasks")
            ?? new();
    }

    public async Task<List<WorkItemResponse>> GetProjectAsync(int projectId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();

        return await client.GetFromJsonAsync<List<WorkItemResponse>>($"{BaseUrl}/projects/{projectId}")
            ?? new();
    }

    public async Task<WorkItemResponse> CreatePersonalAsync(CreatePersonalWorkItemRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/personal", request);

        return await result.Content.ReadFromJsonAsync<WorkItemResponse>()
            ?? throw new Exception("Failed to parse response");
    }

    public async Task<WorkItemResponse> CreateProjectAsync(int projectId, int columnId, CreateProjectWorkItemRequest request)
    {
        // return await result.Content.ReadFromJsonAsync<WorkItemResponse>()
        //     ?? throw new Exception("Failed to parse response");
        var client = await getHttpClient.GetPrivateHttpClient();

        var result = await client.PostAsJsonAsync(
            $"{BaseUrl}/projects/{projectId}/columns/{columnId}", request);

        var text = await result.Content.ReadAsStringAsync();

        Console.WriteLine("STATUS: " + result.StatusCode);
        Console.WriteLine("RESPONSE: " + text);

        if (!result.IsSuccessStatusCode)
            throw new Exception($"Server error: {text}");

        return JsonSerializer.Deserialize<WorkItemResponse>(text)
            ?? throw new Exception("Failed to parse response");
    }

    public async Task<WorkItemResponse> UpdateAsync(int workId, UpdateWorkItemRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PatchAsJsonAsync($"{BaseUrl}/{workId}", request);

            
        var content = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<WorkItemResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task MoveAsync(int taskId, int columnId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();

        await client.PatchAsJsonAsync(
            $"{BaseUrl}/{taskId}/move",
            new MoveWorkItemRequest
            {
                ColumnId = columnId
            });
    }

    public async Task DeleteAsync(int workId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        await client.DeleteAsync($"{BaseUrl}/{workId}");
    }
}
