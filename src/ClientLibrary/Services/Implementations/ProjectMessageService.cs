
using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.ProjectMessageDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class ProjectMessageService(GetHttpClient getHttpClient) : IProjectMessageService
{
    private const string BaseUrl = "api/projects"; 
    public async Task<ProjectMessageResponse> CreateAsync(int projectId, CreateProjectMessageRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/{projectId}/messages", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<ProjectMessageResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<ProjectMessageResponse>> GetMessagesAsync(int projectId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<ProjectMessageResponse>>($"{BaseUrl}/{projectId}/messages")
            ?? new();
    }

    public async Task<ProjectMessageResponse> UpdateAsync(int projectId, int messageId, UpdateProjectMessageRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PutAsJsonAsync($"{BaseUrl}/{projectId}/messages/{messageId}", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<ProjectMessageResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task DeleteAsync(int projectId, int messageId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{projectId}/messages/{messageId}");

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }
    }
}
