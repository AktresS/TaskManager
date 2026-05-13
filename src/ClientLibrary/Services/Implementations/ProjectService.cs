using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.ProjectDtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class ProjectService(GetHttpClient getHttpClient) : IProjectService
{
    private const string BaseUrl = "api/projects";
    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync(BaseUrl, request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<ProjectResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<ProjectResponse>($"{BaseUrl}/{id}") ?? new();
    }

    public async Task<List<ProjectResponse>> GetUserProjectsAsync()
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<ProjectResponse>>(BaseUrl) ?? new();
    }

    public async Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PutAsJsonAsync($"{BaseUrl}/{id}", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<ProjectResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    
    public async Task DeleteAsync(int id)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{id}");

        if (!result.IsSuccessStatusCode){
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }
    }
}
