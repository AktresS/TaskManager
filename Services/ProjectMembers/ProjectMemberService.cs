using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos;
using TaskManager.Models;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.ProjectMembers;

public class ProjectMemberService(AppDbContext context, IProjectAuthorizationService auth) : IProjectMemberService
{
    public async Task<List<ProjectMemberResponse>> GetMembersAsync(int projectId)
    {
        return await context.ProjectMembers
            .Where(x => x.ProjectId == projectId)
            .Select(x => new ProjectMemberResponse
            {
                ProjectMemberId = x.ProjectMemberId,
                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,
                UserId = x.UserId,
                UserName = x.User.Name,
                MemberRole = x.Role
            })
            .ToListAsync();
    }
    public async Task AddMemberAsync(int projectId, AddProjectMemberRequest request)
    {
        var projectExists = await context.Projects
            .AnyAsync(x => x.ProjectId == projectId);

        if (!projectExists)
        {
            throw new Exception("Project not found");
        }

        await auth.EnsureAdmin(projectId);

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = request.UserId,
            Role = request.Role
        };

        context.ProjectMembers.Add(member);

        await context.SaveChangesAsync();
    }

    public async Task<ProjectMemberRoleResponse> UpdateProjectMemberRole(int projectId, int userId, UpdateProjectRoleRequest request)
    {
        var member = await context.ProjectMembers
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

        if (member is null)
            throw new Exception("Project not found");

        await auth.EnsureOwner(projectId);
        
        member.Role = request.Role;

        await context.SaveChangesAsync();

        return new ProjectMemberRoleResponse{
            UserId = member.UserId,
            UserName = member.User?.Name ?? "Who Are U?",
            Role = member.Role
        };
    }

    public async Task RemoveMemberAsync(int projectId, int userId)
    {   
        var member = await context.ProjectMembers.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

        if (member is null)
            throw new Exception("Project or member not found");

        await auth.EnsureAdmin(projectId);

        context.ProjectMembers.Remove(member);
        await context.SaveChangesAsync();
    }
}
