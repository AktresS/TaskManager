using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models;

public class WorkItemMessageRead
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("MessageId")]
    public WorkItemMessage Message { get; set; } = null!;
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
