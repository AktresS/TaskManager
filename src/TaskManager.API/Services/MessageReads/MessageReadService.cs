using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;

namespace TaskManager.Services.MessageReads;

public class MessageReadService(AppDbContext context, ICurrentUserService currentUser) : IMessageReadService
{
    public async Task MarkProjectMessagesReadAsync(int projectId)
    {
        var userId = currentUser.UserId;

        var unreadMessageIds = await context.ProjectMessages
            .Where(m => m.ProjectId == projectId)
            .Where(m => !m.Reads.Any(r => r.UserId == userId))
            .Select(m => m.ProjectMessageId)
            .ToListAsync();

        if (!unreadMessageIds.Any()) return;

        var reads = unreadMessageIds.Select(id => new ProjectMessageRead
        {
            MessageId = id,
            UserId    = userId
        });

        context.ProjectMessageReads.AddRange(reads);
        await context.SaveChangesAsync();
    }

    public async Task MarkWorkItemMessagesReadAsync(int workItemId)
    {
        var userId = currentUser.UserId;

        var unreadMessageIds = await context.WorkItemMessages
            .Where(m => m.WorkItemId == workItemId)
            .Where(m => !m.Reads.Any(r => r.UserId == userId))
            .Select(m => m.WorkItemMessageId)
            .ToListAsync();

        if (!unreadMessageIds.Any()) return;

        var reads = unreadMessageIds.Select(id => new WorkItemMessageRead
        {
            MessageId = id,
            UserId    = userId
        });

        context.WorkItemMessageReads.AddRange(reads);
        await context.SaveChangesAsync();
    }
}
