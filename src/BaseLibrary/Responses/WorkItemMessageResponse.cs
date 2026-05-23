
namespace BaseLibrary.Responses;

public class WorkItemMessageResponse
{
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int WorkItemId { get; set; }
    public string WorkItemName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? Text { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
}
