using System;
using BaseLibrary.Enums;

namespace BaseLibrary.Responses;

public class NotificationResponse
{
    public int NotificationId { get; set; }
    public string Text { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? Link { get; set; }
    public DateTime CreatedAt { get; set; }
}
