using System;
using BaseLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos.WorkItemMessage;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.WorkItemMessages;

public class WorkItemMessageService(AppDbContext context, ICurrentUserService currentUser, IProjectAuthorizationService auth) : IWorkItemMessageService
{
    private async Task<WorkItem> GetAndCheckAccess(int workItemId)
    {
        var item = await context.WorkItems.Include(x => x.Project).FirstOrDefaultAsync(x => x.WorkItemId == workItemId);

        if (item == null)
            throw new Exception("Task not found");
        
        if (item.ProjectId != null)
        {
            await auth.EnsureMember(item.ProjectId.Value);
        }
        else
        {
            if (item.CreatedById != currentUser.UserId)
                throw new Exception("No access to this task");
        }
        
        return item;
    }
    public async Task<WorkItemMessageResponse> CreateAsync(int workItemId, CreateWorkItemMessageRequest request)
    {
        await GetAndCheckAccess(workItemId);

        if (string.IsNullOrWhiteSpace(request.Text) && string.IsNullOrWhiteSpace(request.AttachmentUrl))
            throw new ArgumentException("Message must contain text or attachment");

        var message = new WorkItemMessage
        {
            WorkItemId = workItemId,
            UserId = currentUser.UserId,
            Text = request.Text,
            AttachmentUrl = request.AttachmentUrl
        };

        context.WorkItemMessages.Add(message);
        await context.SaveChangesAsync();

        var createdMessage = await context.WorkItemMessages
            .Include(x => x.User)
            .Include(x => x.WorkItem)
            .FirstOrDefaultAsync(x => x.WorkItemMessageId == message.WorkItemMessageId);

        if (createdMessage == null)
            throw new Exception($"Message {message.WorkItemMessageId} not found");

        return MapToResponse(createdMessage);
    }

    private WorkItemMessageResponse MapToResponse(WorkItemMessage message)
    {
        return new WorkItemMessageResponse
        {
            MessageId = message.WorkItemMessageId,
            UserId = message.UserId,
            UserName = message.User.Name,
            WorkItemId = message.WorkItemId,
            WorkItemName = message.WorkItem.Title,
            Text = message.Text,
            AttachmentUrl = message.AttachmentUrl,
            SentDate = message.SentDate
        };
    }

    public async Task<List<WorkItemMessageResponse>> GetAsync(int workItemId)
    {
        await GetAndCheckAccess(workItemId);

        var messages = await context.WorkItemMessages
            .Include(x => x.User)
            .Where(x => x.WorkItemId == workItemId)
            .OrderByDescending(x => x.SentDate)
            .ToListAsync();

        return messages.Select(MapToResponse).ToList();
    }

    public async Task<WorkItemMessageResponse> UpdateAsync(int workItemId, int messageId, UpdateWorkItemMessageRequest request)
    {
        await GetAndCheckAccess(workItemId);

        var message = await context.WorkItemMessages
            .FirstOrDefaultAsync(x => x.WorkItemId == workItemId && x.WorkItemMessageId == messageId);

        if (message == null)
            throw new ArgumentException("Message not found");

        if (message.UserId != currentUser.UserId)
            throw new UnauthorizedAccessException("You can only edit your own messages");

        if (request.Text != null)
            message.Text = string.IsNullOrWhiteSpace(request.Text) ? null : request.Text;

        if (request.AttachmentUrl != null)
            message.AttachmentUrl = string.IsNullOrWhiteSpace(request.AttachmentUrl) ? null : request.AttachmentUrl;

        if (string.IsNullOrWhiteSpace(message.Text) && string.IsNullOrWhiteSpace(message.AttachmentUrl))
        {
            throw new ArgumentException("Message must contain text or attachment");
        }

        await context.SaveChangesAsync();

        var updatedMessage = await context.WorkItemMessages
            .Include(x => x.WorkItem)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.WorkItemMessageId == message.WorkItemMessageId);
        
        if (updatedMessage == null)
            throw new Exception("Message not found");

        return MapToResponse(updatedMessage);
    }

    public async Task Delete(int workItemId, int messageId)
    {
        var item = await GetAndCheckAccess(workItemId);

        var message = await context.WorkItemMessages.FirstOrDefaultAsync(x => x.WorkItemId == workItemId && x.WorkItemMessageId == messageId);

        if (message == null)
            throw new ArgumentException("Message not found");

        var isOwner = message.UserId == currentUser.UserId;

        if (item.ProjectId != null)
        {
            var role = await auth.GetUserRole(item.ProjectId.Value);

            if (!isOwner && role != MemberRole.Owner && role != MemberRole.Admin)
                throw new UnauthorizedAccessException("No rights to delete message");
        }
        else
        {
            if (!isOwner)
                throw new UnauthorizedAccessException("You can delete only your messages");
        }

        context.WorkItemMessages.Remove(message);
        await context.SaveChangesAsync();

    }
}
