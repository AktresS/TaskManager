using System;

namespace TaskManager.Models;

public class WorkItemAssignee
{
    public int WorkItemAssigneeId { get; set; }
    public int WorkItemId { get; set; }
    public int UserId { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    public WorkItem WorkItem { get; set; } = null!;
    public User User { get; set; } = null!;
}
