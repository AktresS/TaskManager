
using TaskManager.Dtos.WorkItemAssignee;

namespace TaskManager.Services.WorkItemAssignees;

public interface IWorkItemAssigneeService
{
    Task AddAsync(int workItemId, AddAssigneeRequest request);
    Task<List<WorkItemAssigneeResponse>> GetAsync(int workItemId);
    Task RemoveAsync(int workItemId, int userId);
}
