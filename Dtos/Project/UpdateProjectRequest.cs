using System;

namespace TaskManager.Dtos.Project;

public class UpdateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
