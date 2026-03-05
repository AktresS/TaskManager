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


    //Навигационные свойства
    //Проекты, созданные пользователем
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

    //Проекты, в которых состоит пользователь
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();

    //Задачи, созданные пользователем
    public ICollection<WorkItem> CreatedWorkItems { get; set; } = new List<WorkItem>();

    //Задачи, назначенные пользователю
    public ICollection<WorkItemAssignee> AssignedWorkItems { get; set; } = new List<WorkItemAssignee>();

    //Сообщения
    public ICollection<ProjectMessage> ProjectMessages { get; set; } = new List<ProjectMessage>();
    public ICollection<WorkItemMessage> WorkItemMessages { get; set; } = new List<WorkItemMessage>();
}
