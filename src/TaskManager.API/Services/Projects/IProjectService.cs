using TaskManager.Dtos.Project;

namespace TaskManager.Services.Projects;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request);
    Task<List<ProjectResponse>> GetUserProjectsAsync();
    Task<ProjectResponse?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}
