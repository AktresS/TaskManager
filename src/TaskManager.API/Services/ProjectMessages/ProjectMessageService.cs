using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos.ProjectMessage;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.ProjectMessages;

public class ProjectMessageService(AppDbContext context, ICurrentUserService currentUser, IProjectAuthorizationService auth): IProjectMessageService
{
    public async Task<ProjectMessageResponse> CreateAsync(int projectId, CreateProjectMessageRequest request)
    {
        await auth.EnsureMember(projectId);

        var message = new ProjectMessage
        {
            ProjectId = projectId,
            UserId = currentUser.UserId,
            Text = request.Text,
            AttachmentUrl = request.AttachmentUrl
        };

        context.ProjectMessages.Add(message);
        await context.SaveChangesAsync();

        return new ProjectMessageResponse
        {
            Id = message.ProjectMessageId,
            UserId = message.UserId,
            UserName = (await context.Users.FindAsync(message.UserId))!.Name,
            Text = message.Text,
            AttachmentUrl = message.AttachmentUrl,
            SentDate = message.SentDate
        };
    }
    
    public async Task<ProjectMessageResponse> UpdateAsync(int projectId, int messageId, UpdateProjectMessageRequest request)
    {
        await auth.EnsureMember(projectId);

        var message = await context.ProjectMessages
            .Include(x => x.Project)
            .Include(x => x.User)
            .FirstOrDefaultAsync(m => m.ProjectMessageId == messageId && m.ProjectId == projectId);

        if (message == null)
            throw new ArgumentException("Message not found in this project");

        if (message.UserId != currentUser.UserId)
            throw new UnauthorizedAccessException("You can only edit your own messages");

        message.Text = request.Text;
        message.AttachmentUrl = request.AttachmentUrl;

        await context.SaveChangesAsync();

        return new ProjectMessageResponse
        {
            Id = message.ProjectMessageId,
            UserId = message.UserId,
            UserName = message.User.Name,
            ProjectId = message.ProjectId,
            ProjectName = message.Project.Name,
            Text = message.Text,
            AttachmentUrl = message.AttachmentUrl,
            SentDate = message.SentDate
        };
        
    }

    public async Task<List<ProjectMessageResponse>> GetMessagesAsync(int projectId)
    {
        await auth.EnsureMember(projectId);

        return await context.ProjectMessages
            .Where(p => p.ProjectId == projectId)
            .OrderBy(x => x.SentDate)
            .Select(p => new ProjectMessageResponse
            {
                Id = p.ProjectMessageId,
                UserId = p.UserId,
                UserName = p.User.Name,
                ProjectId = p.ProjectId,
                ProjectName = p.Project.Name,
                Text = p.Text,
                AttachmentUrl = p.AttachmentUrl,
                SentDate = p.SentDate
            }).ToListAsync();
    }

    public async Task DeleteAsync(int projectId, int messageId)
    {   
        await auth.EnsureMember(projectId);

        var message = await context.ProjectMessages.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.ProjectMessageId == messageId);

        if (message == null)
            throw new ArgumentException("Message not found in this project");

        if (message.UserId != currentUser.UserId)
            throw new UnauthorizedAccessException("You can only edit your own messages");

        context.ProjectMessages.Remove(message);
        await context.SaveChangesAsync();

    }
}
