using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models;

public class Project
{
    [Key]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(130)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? Description { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsActive { get; set; } = true;

    [ForeignKey("CreatedById")]
    public User CreatedBy { get; set; } = null!;

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();

    public ICollection<ProjectMessage> Messages { get; set; } = new List<ProjectMessage>();
}
