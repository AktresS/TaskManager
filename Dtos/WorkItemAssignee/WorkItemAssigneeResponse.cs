
namespace TaskManager.Dtos.WorkItemAssignee;

public class WorkItemAssigneeResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int WorkItemId { get; set; }
    public string WorkItemTitle { get; set; } = string.Empty;
}
