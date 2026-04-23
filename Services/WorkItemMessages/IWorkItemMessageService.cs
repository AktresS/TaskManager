
using TaskManager.Dtos.WorkItemMessage;

namespace TaskManager.Services.WorkItemMessages;

public interface IWorkItemMessageService
{
    Task<WorkItemMessageResponse> CreateAsync(int workItemId, CreateWorkItemMessageRequest request);
    Task<List<WorkItemMessageResponse>> GetAsync(int workItemId);
    Task<WorkItemMessageResponse> UpdateAsync(int workItemId, int messageId, UpdateWorkItemMessageRequest request);
    Task Delete(int workItemId, int messageId);
}
