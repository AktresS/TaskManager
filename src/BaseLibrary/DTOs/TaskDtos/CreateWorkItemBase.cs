
using BaseLibrary.Enums;

namespace BaseLibrary.DTOs.TaskDtos;

public class CreateWorkItemBase
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DeadLine { get; set; }
}
