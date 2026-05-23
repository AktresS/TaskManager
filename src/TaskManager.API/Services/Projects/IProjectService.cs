using BaseLibrary.DTOs.ProjectDtos;
using BaseLibrary.Responses;

namespace TaskManager.Services.Projects;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request);
    Task<List<ProjectResponse>> GetUserProjectsAsync();
    Task<ProjectResponse?> GetByIdAsync(int id);
    Task<bool> TogglePinAsync(int projectId);
    Task DeleteAsync(int id);
}
