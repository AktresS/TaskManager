using BaseLibrary.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Hubs;
using TaskManager.Models;
using TaskManager.Services.Notifications;

public class NotificationSender(AppDbContext context, IHubContext<NotificationHub> hub) : INotificationSender
{
    public async Task SendAsync(int userId, string text, NotificationType type, string? link = null)
    {
        var settings = await context.UserSettings
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (settings is not null)
        {
            if (!settings.NotificationsEnabled) return;

            if (type == NotificationType.DeadlineReminder && !settings.DeadlineNotificationsEnabled) return;
            if (type is NotificationType.NewProjectMessage or NotificationType.NewWorkItemMessage 
                && !settings.MessageNotificationsEnabled) return;
        }

        var notification = new Notification
        {
            UserId = userId,
            Text = text,
            Type = type,
            Link = link
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        await hub.Clients
            .Group($"user_{userId}")
            .SendAsync("ReceiveNotification", new
            {
                notification.NotificationId,
                notification.Text,
                notification.Type,
                notification.Link,
                notification.CreatedAt
            });
    }
}
