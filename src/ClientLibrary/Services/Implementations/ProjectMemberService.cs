using System;
using System.Net.Http.Json;
using System.Text.Json;
using BaseLibrary.DTOs.ProjectMemberDtos;
using BaseLibrary.Enums;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class ProjectMemberService(GetHttpClient getHttpClient) : IProjectMemberService
{
    private const string BaseUrl = "api/projects";
    public async Task AddMemberAsync(int projectId, AddProjectMemberRequest request)
    {
        var client  = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PostAsJsonAsync($"{BaseUrl}/{projectId}/members", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);
    }

    public async Task<MemberRole?> GetMyRoleAsync(int projectId)
    {
        var client = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<MemberRole?>($"{BaseUrl}/{projectId}/members/my-role");
    }

    public async Task<List<ProjectMemberResponse>> GetMembersAsync(int projectId)
    {
        var client  = await getHttpClient.GetPrivateHttpClient();
        return await client.GetFromJsonAsync<List<ProjectMemberResponse>>($"{BaseUrl}/{projectId}/members")
            ?? new();
    }

    public async Task<ProjectMemberRoleResponse> UpdateProjectMemberRole(int projectId, int userId, UpdateProjectRoleRequest request)
    {
        var client  = await getHttpClient.GetPrivateHttpClient();
        var result = await client.PutAsJsonAsync($"{BaseUrl}/{projectId}/members/{userId}", request);

        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception(content);
        
        return JsonSerializer.Deserialize<ProjectMemberRoleResponse>(content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task RemoveMemberAsync(int projectId, int userId)
    {
        var client  = await getHttpClient.GetPrivateHttpClient();
        var result = await client.DeleteAsync($"{BaseUrl}/{projectId}/members/{userId}");

        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete project: {error}");
        }

    }
}
