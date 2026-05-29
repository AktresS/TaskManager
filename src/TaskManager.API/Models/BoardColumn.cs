using System;
using BaseLibrary.Enums;

namespace TaskManager.Models;

public class BoardColumn
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;

    public List<WorkItem> WorkItems { get; set; } = new();
    public TaskState? AutoStatus { get; set; }
}
