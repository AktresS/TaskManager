using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Enums;

namespace TaskManager.Models;

public class WorkItemMessage
{
    [Key]
    public int WorkItemMessageId { get; set; }

    [Required]
    public int WorkItemId { get; set; }

    [Required]
    public int UserId { get; set; }

    public string? Text { get; set; }

    public MessageType MessageType { get; set; } = MessageType.User;

    [MaxLength(500)]
    public string? AttachmentUrl { get; set; }

    [Required]
    public DateTime SentDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("WorkItemId")]
    public WorkItem WorkItem { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

}
