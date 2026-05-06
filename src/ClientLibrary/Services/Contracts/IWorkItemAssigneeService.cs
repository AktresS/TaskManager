using BaseLibrary.DTOs.AssigneeDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IWorkItemAssigneeService
{
    Task AddAsync(int workItemId, AddAssigneeRequest request);
    Task<List<WorkItemAssigneeResponse>> GetAsync(int workItemId);
    Task RemoveAsync(int workItemId, int userId);
}
