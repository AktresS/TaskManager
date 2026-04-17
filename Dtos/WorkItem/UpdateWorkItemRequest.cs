using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManager.Enums;

namespace TaskManager.Dtos.WorkItem;

public class UpdateWorkItemRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskPriority? Priority { get; set; }
    public TaskState? State { get; set; }
    public DateTime? DeadLine { get; set; }
}
