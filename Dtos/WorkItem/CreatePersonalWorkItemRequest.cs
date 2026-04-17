
using TaskManager.Enums;

namespace TaskManager.Dtos.WorkItem;

public class CreatePersonalWorkItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DeadLine { get; set; }
}
