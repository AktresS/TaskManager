namespace TaskManager.Dtos.ProjectMessage;

public class ProjectMessageResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Text { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
}
