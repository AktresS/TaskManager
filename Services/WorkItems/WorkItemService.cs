using System;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos.WorkItem;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.WorkItems;

public class WorkItemService(AppDbContext context, ICurrentUserService currentUser, IProjectAuthorizationService auth) : IWorkItemService
{
    public async Task<WorkItemResponse> CreatePersonalAsync(CreatePersonalWorkItemRequest request)
    {
        var item = new WorkItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DeadLine = DateTime.SpecifyKind(request.DeadLine, DateTimeKind.Utc),
            CreatedById = currentUser.UserId,
            ProjectId = null
        };

        context.WorkItems.Add(item);
        await context.SaveChangesAsync();

        return await GetByIdAsync(item.WorkItemId);
    }

    public async Task<WorkItemResponse> CreateProjectAsync(int projectId, CreateProjectWorkItemRequest request)
    {
        await auth.EnsureMember(projectId);

        var assigneesToAdd = new List<WorkItemAssignee>();

        if (request.AssigneeIds != null && request.AssigneeIds.Any())
        {   
            foreach (var userId in request.AssigneeIds)
            {
                var isMember = await context.ProjectMembers
                    .AnyAsync(x => x.ProjectId == projectId && x.UserId == userId);
                
                if (!isMember)
                    throw new Exception($"User {userId} is not a project member");

                assigneesToAdd.Add(new WorkItemAssignee
                {
                    UserId = userId
                });
            }
        }
        else
        {
            assigneesToAdd.Add(new WorkItemAssignee
            {
                UserId = currentUser.UserId
            });
        }

        var item = new WorkItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DeadLine = DateTime.SpecifyKind(request.DeadLine, DateTimeKind.Utc),
            CreatedById = currentUser.UserId,
            ProjectId = projectId
        };

        foreach (var a in assigneesToAdd)
        {
            a.WorkItemId = item.WorkItemId;
        }

        context.WorkItems.Add(item);
        await context.SaveChangesAsync();

        context.WorkItemAssignees.AddRange(assigneesToAdd);
        await context.SaveChangesAsync();

        return await GetByIdAsync(item.WorkItemId);
    }

    public async Task<List<WorkItemResponse>> GetMyItemAsync()
    {
        var userId = currentUser.UserId;

        var items = await context.WorkItems
            .Include(x => x.Assignees)
            .Include(x => x.CreatedBy)
            .Include(x => x.Project)
            .Where(x => x.CreatedById == userId || x.Assignees.Any(x => x.UserId == userId))
            .OrderByDescending(x => x.DeadLine)
            .ToListAsync();
        
        return items.Select(MapToResponse).ToList();
    }

    public async Task<WorkItemResponse> GetByIdAsync(int id)
    {
        var item = await context.WorkItems
        .Include(w => w.Assignees)
        .Include(w => w.Project)
        .Include(w => w.CreatedBy)
        .FirstOrDefaultAsync(w => w.WorkItemId == id);

        if (item == null)
            throw new Exception($"WorkItem {id} not found");

        if (item.ProjectId != null)
        {
            await auth.EnsureMember(item.ProjectId.Value);
        }
        else
        {
            if (item.CreatedById != currentUser.UserId)
            {
                throw new UnauthorizedAccessException("No access to this task");
            }
        }

        return MapToResponse(item);
    }

    public async Task<List<WorkItemResponse>> GetProjectTasksAsync(int projectId)
    {
        await auth.EnsureMember(projectId);

        var items = await context.WorkItems
            .Include(x => x.Assignees)
            .Include(x => x.CreatedBy)
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
            
        return items.Select(MapToResponse).ToList();
    }

    private WorkItemResponse MapToResponse(WorkItem item)
    {
        return new WorkItemResponse
        {
            Id = item.WorkItemId,
            Title = item.Title,
            Description = item.Description,
            Priority = item.Priority,
            State = item.State,
            DeadLine = item.DeadLine,
            ProjectId = item.ProjectId,
            ProjectName = item.Project?.Name,
            CreatedById = item.CreatedById,
            CreatedByName = item.CreatedBy.Name,
            AssigneeIds = item.ProjectId == null ? new List<int> { item.CreatedById } : item.Assignees.Select(a => a.UserId).ToList()
        };
    }

    public async Task<WorkItemResponse> UpdateAsync(int id, UpdateWorkItemRequest request)
    {
        var item = await context.WorkItems
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.WorkItemId == id);

        if (item == null)
            throw new Exception("Task not found");

        var isCreator = item.CreatedById == currentUser.UserId;

        if (item.ProjectId != null)
        {
            
            var isAssignee = item.Assignees.Any(x => x.UserId == currentUser.UserId);
            var role = await auth.GetUserRole(item.ProjectId.Value);

            if (!isCreator && !isAssignee && role != MemberRole.Owner && role != MemberRole.Admin)
                throw new UnauthorizedAccessException("No rights to update this task");

        }
        else
        {   
            if (!isCreator)
                throw new UnauthorizedAccessException("You can only update your own task");
        }

        if (request.Title != null)
            item.Title = request.Title;

        if (request.Description != null)
            item.Description = request.Description;

        if (request.Priority.HasValue)
            item.Priority = request.Priority.Value;
        
        if (request.State.HasValue)
            item.State = request.State.Value;

        if (request.DeadLine.HasValue)
            item.DeadLine = DateTime.SpecifyKind(request.DeadLine.Value, DateTimeKind.Utc);
        
        item.UpdateDate = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return await GetByIdAsync(item.WorkItemId);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await context.WorkItems
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.WorkItemId == id);

        if (item == null)
            throw new Exception("Task not found");

        var isCreator = item.CreatedById == currentUser.UserId;

        if (item.ProjectId != null)
        {
            var role = await auth.GetUserRole(item.ProjectId.Value);

            if (!isCreator && role != MemberRole.Owner && role != MemberRole.Admin)
                throw new UnauthorizedAccessException("No rights to delete this task");
        }
        else
        {
            if (!isCreator)
                throw new UnauthorizedAccessException("You can only delete your own task");
        }

        context.WorkItems.Remove(item);
        await context.SaveChangesAsync();
    }

}
