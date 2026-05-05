using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs.ProjectDtos;

public class CreateProjectRequest
{
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
