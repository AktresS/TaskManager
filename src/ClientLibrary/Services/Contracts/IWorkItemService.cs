
using BaseLibrary.DTOs.TaskDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IWorkItemService
{
    Task<List<WorkItemResponse>> GetMyAsync();
    Task<List<WorkItemResponse>> GetProjectAsync(int projectId);

    Task<WorkItemResponse> CreatePersonalAsync(CreatePersonalWorkItemRequest request);
    Task<WorkItemResponse> CreateProjectAsync(int projectId, int columnId, CreateProjectWorkItemRequest request);

    Task<WorkItemResponse> UpdateAsync(int id, UpdateWorkItemRequest request);
    Task MoveAsync(int taskId, int columnId);
    Task DeleteAsync(int id);
}
