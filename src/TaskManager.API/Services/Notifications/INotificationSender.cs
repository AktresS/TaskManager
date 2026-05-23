
using BaseLibrary.Enums;

namespace TaskManager.Services.Notifications;

public interface INotificationSender
{
    Task SendAsync(int userId, string text, NotificationType type, string? link = null);
}
