using BaseLibrary.DTOs.ProjectMessageDtos;
using BaseLibrary.Enums;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.Notifications;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.ProjectMessages;

public class ProjectMessageService(AppDbContext context, ICurrentUserService currentUser, IProjectAuthorizationService auth, INotificationSender sender): IProjectMessageService
{
    public async Task<ProjectMessageResponse> CreateAsync(int projectId, CreateProjectMessageRequest request)
    {
        await auth.EnsureMember(projectId);

        var message = new ProjectMessage
        {
            ProjectId = projectId,
            UserId = currentUser.UserId,
            Text = request.Text,
            AttachmentUrl = request.AttachmentUrl,
            AttachmentName = request.AttachmentName
        };

        context.ProjectMessages.Add(message);
        await context.SaveChangesAsync();

        var members = await context.ProjectMembers
            .Where(x => x.ProjectId == projectId && x.UserId != currentUser.UserId)
            .ToListAsync();

        var project = await context.Projects.FindAsync(projectId);

        var sender_user = await context.Users.FindAsync(currentUser.UserId);
        foreach (var member in members)
        {
            await sender.SendAsync(
                member.UserId,
                $"{sender_user!.Name} написал в чате проекта «{project!.Name}»",
                NotificationType.NewProjectMessage,
                $"/chats?project={projectId}");
        }

        return new ProjectMessageResponse
        {
            Id = message.ProjectMessageId,
            UserId = message.UserId,
            UserName = (await context.Users.FindAsync(message.UserId))!.Name,
            Text = message.Text,
            AttachmentUrl = message.AttachmentUrl,
            AttachmentName = request.AttachmentName,
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
            .Include(p => p.User)
            .Include(p => p.Project)
            .Include(p => p.Reads)
            .Where(p => p.ProjectId == projectId)
            .OrderBy(x => x.SentDate)
            .Select(p => new ProjectMessageResponse
            {
                Id = p.ProjectMessageId,
                UserId = p.UserId,
                UserName = p.User.Name,
                ProjectId = p.ProjectId,
                ProjectName = p.Project.Name,
                AvatarUrl = p.User.AvatarUrl,
                Text = p.Text,
                AttachmentUrl = p.AttachmentUrl,
                AttachmentName = p.AttachmentName,
                SentDate = p.SentDate,
                ReadByUserIds  = p.Reads.Select(r => r.UserId).ToList()
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
