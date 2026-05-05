using System;

namespace BaseLibrary.DTOs.TaskDtos;

public class CreateProjectWorkItemRequest : CreateWorkItemBase
{
    public List<int> AssigneeIds { get; set; } = new();
    public int ColumnId { get; set; } 
}
