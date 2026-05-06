
using System.Net.Http.Json;
using BaseLibrary.DTOs.AssigneeDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class WorkItemAssigneeService(GetHttpClient getHttpClient) : IWorkItemAssigneeService
{
    private const string BaseUrl = "api/workitems";
    public async Task AddAsync(int workItemId, AddAssigneeRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/{workItemId}/assignees", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);
    }

    public async Task<List<WorkItemAssigneeResponse>> GetAsync(int workItemId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<WorkItemAssigneeResponse>>($"{BaseUrl}/{workItemId}/assignees")
            ?? new();
    }

    public async Task RemoveAsync(int workItemId, int userId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{workItemId}/assignees/{userId}");

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }
    }
}
