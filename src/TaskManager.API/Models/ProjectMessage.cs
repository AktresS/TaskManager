using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models;

public class ProjectMessage
{
    [Key]
    public int ProjectMessageId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int UserId { get; set; }

    public string? Text { get; set; }

    [MaxLength(500)]
    public string? AttachmentUrl { get; set; }

    [Required]
    public DateTime SentDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
