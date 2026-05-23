using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models;

public class UserSettings
{
    [Key]
    public int UserSettingsId { get; set; }

    [Required]
    public int UserId { get; set; }

    public bool NotificationsEnabled { get; set; } = true;
    public bool DeadlineNotificationsEnabled { get; set; } = true;
    public bool MessageNotificationsEnabled { get; set; } = true;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}