using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseLibrary.Enums;

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

    [MaxLength(255)]
    public string? AttachmentName { get; set; }

    [Required]
    public DateTime SentDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("WorkItemId")]
    public WorkItem WorkItem { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    public ICollection<WorkItemMessageRead> Reads { get; set; } = new List<WorkItemMessageRead>();
}
