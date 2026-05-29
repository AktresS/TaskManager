
using BaseLibrary.DTOs.ProjectMessageDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IProjectMessageService
{
    Task<ProjectMessageResponse> CreateAsync(int projectId, CreateProjectMessageRequest request);
    Task<ProjectMessageResponse> UpdateAsync(int projectId, int messageId, UpdateProjectMessageRequest request);
    Task<List<ProjectMessageResponse>> GetMessagesAsync(int projectId);
    Task MarkReadAsync(int id);
    Task DeleteAsync(int projectId, int messageId);
}
