namespace BaseLibrary.Responses;

public class ProjectMessageResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? Text { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
    public List<int> ReadByUserIds { get; set; } = new();
}
