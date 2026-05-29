
using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.WorkItemMessageDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class WorkItemMessageService(GetHttpClient getHttpClient) : IWorkItemMessageService
{
    private const string BaseUrl = "api/workitems";
    public async Task<WorkItemMessageResponse> CreateAsync(int workItemId, CreateWorkItemMessageRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/{workItemId}/messages", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<WorkItemMessageResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<WorkItemMessageResponse>> GetAsync(int workItemId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<WorkItemMessageResponse>>($"{BaseUrl}/{workItemId}/messages")
            ?? new();
    }

    public async Task<WorkItemMessageResponse> UpdateAsync(int workItemId, int messageId, UpdateWorkItemMessageRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PatchAsJsonAsync($"{BaseUrl}/{workItemId}/messages/{messageId}", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<WorkItemMessageResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task MarkReadAsync(int workItemId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        await client.PostAsync($"{BaseUrl}/{workItemId}/messages/read", null);
    }

    public async Task Delete(int workItemId, int messageId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{workItemId}/messages/{messageId}");

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }
    }
}
