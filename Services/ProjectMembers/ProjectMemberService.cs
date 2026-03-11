using System;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos;
using TaskManager.Models;

namespace TaskManager.Services.ProjectMembers;

public class ProjectMemberService(AppDbContext context) : IProjectMemberService
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

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = request.UserId,
            Role = request.Role
        };

        context.ProjectMembers.Add(member);

        await context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int projectId, int userId)
    {
        var member = await context.ProjectMembers.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

        if (member is null)
            throw new Exception("Member not found");

        context.ProjectMembers.Remove(member);
        await context.SaveChangesAsync();
    }
}
