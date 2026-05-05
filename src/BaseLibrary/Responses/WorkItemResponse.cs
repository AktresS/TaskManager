using System;
using BaseLibrary.DTOs;
using BaseLibrary.Enums;

namespace BaseLibrary.Responses;

public class WorkItemResponse
{
    public int Id { get; set; }
    public int CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty; 
    public int? ProjectId { get; set; }
    public string? ProjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int? ColumnId { get; set; } 
    public TaskPriority Priority { get; set; }
    public TaskState State { get; set; }
    public DateTime DeadLine { get; set; }
    public List<UserShortDto> Assignees { get; set; } = new();
}
