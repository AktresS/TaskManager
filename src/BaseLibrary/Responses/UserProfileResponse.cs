using System;

namespace BaseLibrary.Responses;

public class UserProfileResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool DeadlineNotificationsEnabled { get; set; }
    public bool MessageNotificationsEnabled { get; set; }
}
