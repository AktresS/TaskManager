using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Enums;

namespace TaskManager.Models;

public class ProjectMember
{
    [Key]
    public int ProjectMemberId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public MemberRole Role { get; set; }

    [Required]
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
