
using BaseLibrary.DTOs.ProjectMemberDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IProjectMemberService
{
    Task<List<ProjectMemberResponse>> GetMembersAsync(int projectId);
    Task AddMemberAsync(int projectId, AddProjectMemberRequest request);
    Task<ProjectMemberRoleResponse> UpdateProjectMemberRole(int projectId, int userId, UpdateProjectRoleRequest request);
    Task RemoveMemberAsync(int projectId, int UserId);
}
