using TaskManager.Dtos;

namespace TaskManager.Services.ProjectMembers;

public interface IProjectMemberService
{
    Task<List<ProjectMemberResponse>> GetMembersAsync(int projectId);
    Task AddMemberAsync(int projectId, AddProjectMemberRequest request);
    Task<ProjectMemberRoleResponse> UpdateProjectMemberRole(int projectId, int userId, UpdateProjectRoleRequest request);
    Task RemoveMemberAsync(int projectId, int UserId);
}
