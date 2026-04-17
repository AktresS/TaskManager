using TaskManager.Dtos.WorkItem;

namespace TaskManager.Services.WorkItems;

public interface IWorkItemService
{
    Task<WorkItemResponse> CreatePersonalAsync(CreatePersonalWorkItemRequest request);
    Task<WorkItemResponse> CreateProjectAsync(int projectId, CreateProjectWorkItemRequest request);
    Task<List<WorkItemResponse>> GetMyItemAsync();
    Task<WorkItemResponse> GetByIdAsync(int id);
    Task<List<WorkItemResponse>> GetProjectTasksAsync(int id);
    Task<WorkItemResponse> UpdateAsync(int id, UpdateWorkItemRequest request);
    Task DeleteAsync(int id);
}
