using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Url]
    [MaxLength(400)]
    public string? AvatarUrl { get; set; }

    public UserSettings? Settings { get; set; }

    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public ICollection<WorkItem> CreatedWorkItems { get; set; } = new List<WorkItem>();
    public ICollection<WorkItemAssignee> AssignedWorkItems { get; set; } = new List<WorkItemAssignee>();
    public ICollection<ProjectMessage> ProjectMessages { get; set; } = new List<ProjectMessage>();
    public ICollection<WorkItemMessage> WorkItemMessages { get; set; } = new List<WorkItemMessage>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserPinnedProject> PinnedProjects { get; set; } = new List<UserPinnedProject>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
