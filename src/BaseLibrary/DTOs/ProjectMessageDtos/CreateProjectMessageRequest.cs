namespace BaseLibrary.DTOs.ProjectMessageDtos;

public class CreateProjectMessageRequest
{
    public string? Text { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
}
