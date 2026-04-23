namespace TaskManager.Dtos.WorkItemMessage;

public class CreateWorkItemMessageRequest
{
    public string? Text { get; set; }
    public string? AttachmentUrl { get; set; }
}
