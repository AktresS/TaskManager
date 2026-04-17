using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos.WorkItemAssignee;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.WorkItemAssignees;

public class WorkItemAssigneeService(AppDbContext context, ICurrentUserService currentUser, IProjectAuthorizationService auth) : IWorkItemAssigneeService
{
    public async Task AddAsync(int workItemId, AddAssigneeRequest request)
    {
        var item = await context.WorkItems
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.WorkItemId == workItemId);

        if (item == null)
            throw new Exception("Task not found");

        if (item.ProjectId == null)
            throw new Exception("Cannot assign users to personal task");

        var role = await auth.GetUserRole(item.ProjectId.Value);

        var isCreator = item.CreatedById == currentUser.UserId;

        if (!isCreator && role != MemberRole.Owner && role != MemberRole.Admin)
            throw new UnauthorizedAccessException("No rights to assign users");


        foreach (var userId in request.UserId)
        {
            var isMember = await context.ProjectMembers
                .AnyAsync(x => x.ProjectId == item.ProjectId && x.UserId == userId);
        
            if (!isMember)
                throw new Exception("User is not a project member");

            var alreadyAssigned = item.Assignees
                .Any(x => x.UserId == userId);
            
            if (!alreadyAssigned)
            {
                context.WorkItemAssignees.Add(new WorkItemAssignee
                {
                    WorkItemId = workItemId,
                    UserId = userId
                });
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<WorkItemAssigneeResponse>> GetAsync(int workItemId)
    {
        var item = await context.WorkItems
            .Include(x => x.Assignees)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.WorkItemId == workItemId);

        if (item == null)
            throw new Exception("Task not found");

        if (item.ProjectId == null)
            throw new Exception("Personal task has no assignees");

        await auth.EnsureMember(item.ProjectId.Value);

        return item.Assignees.Select(x => new WorkItemAssigneeResponse
        {
            UserId = x.UserId,
            UserName = x.User.Name,
            WorkItemId = x.WorkItemId,
            WorkItemTitle = x.WorkItem.Title
        }).ToList();
    }

    public async Task RemoveAsync(int workItemId, int userId)
    {
        var item = await context.WorkItems
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.WorkItemId == workItemId);

        if (item == null)
            throw new Exception("Task not found");

        if (item.ProjectId == null)
            throw new Exception("Cannot assign users to personal task");

        var role = await auth.GetUserRole(item.ProjectId.Value);

        var isCreator = item.CreatedById == currentUser.UserId;

        if (!isCreator && role != MemberRole.Owner && role != MemberRole.Admin)
            throw new UnauthorizedAccessException("No rights to assign users");

        var assignee = item.Assignees
            .FirstOrDefault(x => x.UserId == userId);

        if (assignee == null)
            throw new Exception("Assignee not found");

        context.WorkItemAssignees.Remove(assignee);

        await context.SaveChangesAsync();
    }
}
