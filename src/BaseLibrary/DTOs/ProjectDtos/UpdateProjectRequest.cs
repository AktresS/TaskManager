using System;

namespace BaseLibrary.DTOs.ProjectDtos;

public class UpdateProjectRequest
{
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
