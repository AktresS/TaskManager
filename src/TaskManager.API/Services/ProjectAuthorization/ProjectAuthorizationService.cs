using System;
using BaseLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Services.CurrentUser;

namespace TaskManager.Services.ProjectAuthorization;

public class ProjectAuthorizationService(AppDbContext context, ICurrentUserService currentUser) : IProjectAuthorizationService
{
    public async Task<MemberRole> GetUserRole(int projectId)
    {
        var member = await context.ProjectMembers.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == currentUser.UserId);

        if (member == null)
            throw new UnauthorizedAccessException("Not a project member");

        return member.Role;
    }
    public async Task EnsureMember(int projectId)
    {
        await GetUserRole(projectId);
    }
    public async Task EnsureAdmin(int projectId)
    {
        var role = await GetUserRole(projectId);

        if (role != MemberRole.Admin && role != MemberRole.Owner)
            throw new UnauthorizedAccessException("Admin access required");
    }

    public async Task EnsureOwner(int projectId)
    {
        var role = await GetUserRole(projectId);
        
        if (role != MemberRole.Owner)  
            throw new UnauthorizedAccessException("Owner access required");
    }
}
