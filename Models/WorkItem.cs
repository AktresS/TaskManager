using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.SignalR;
using TaskManager.Enums;

namespace TaskManager.Models;

public class WorkItem
{
    [Key]
    public int WorkItemId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskState State { get; set; } = TaskState.New;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Required]
    public int CreatedById { get; set; }

    public int? ProjectId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime DeadLine { get; set; } 

    [ForeignKey("CreatedById")]
    public User CreatedBy { get; set; } = null!;

    [ForeignKey("ProjectId")]
    public Project? Project { get; set; }

    public ICollection<WorkItemAssignee> Assignees { get; set; } = new List<WorkItemAssignee>();
    
    public ICollection<WorkItemMessage> Messages { get; set; } = new List<WorkItemMessage>();
}
