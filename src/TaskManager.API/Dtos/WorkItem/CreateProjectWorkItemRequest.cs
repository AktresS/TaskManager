using System;
using TaskManager.Enums;

namespace TaskManager.Dtos.WorkItem;

public class CreateProjectWorkItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DeadLine { get; set; }

    public List<int> AssigneeIds { get; set; } = null!;
}
