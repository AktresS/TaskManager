using BaseLibrary.DTOs.WorkItemMessageDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IWorkItemMessageService
{
    Task<WorkItemMessageResponse> CreateAsync(int workItemId, CreateWorkItemMessageRequest request);
    Task<List<WorkItemMessageResponse>> GetAsync(int workItemId);
    Task<WorkItemMessageResponse> UpdateAsync(int workItemId, int messageId, UpdateWorkItemMessageRequest request);
    Task Delete(int workItemId, int messageId);
}
