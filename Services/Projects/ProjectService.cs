using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dtos.Project;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;

namespace TaskManager.Services.Projects;

public class ProjectService(AppDbContext context, ICurrentUserService currentUser) : IProjectService
{
    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            CreatedById = currentUser.UserId,
            CreatedDate = DateTime.UtcNow
        };

        context.Projects.Add(project);

        context.ProjectMembers.Add(new ProjectMember
        {
            Project = project,
            UserId = currentUser.UserId,
            Role = Enums.MemberRole.Owner
        });

        await context.SaveChangesAsync();

        return new ProjectResponse
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.CreatedById,
            CreatedAt = project.CreatedDate
        };
    }
    
    public async Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request)
    {
        var project = await context.Projects.FindAsync(id);

        if (project is null)
            throw new Exception("Project not found");

        project.Name = request.Name;
        project.Description = request.Description;

        await context.SaveChangesAsync();

        return new ProjectResponse
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.CreatedById,
            CreatedAt = project.CreatedDate
        };
    }

    public async Task<List<ProjectResponse>> GetUserProjectsAsync()
    {
        return await context.Projects.Select(x => new ProjectResponse
        {
            ProjectId = x.ProjectId,
            Name = x.Name,
            Description = x.Description,
            OwnerId = x.CreatedById,
            CreatedAt = x.CreatedDate
        }).ToListAsync();
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        return await context.Projects
            .Where(x => x.ProjectId == id)
            .Select(x => new ProjectResponse
            {
                ProjectId = x.ProjectId,
                Name = x.Name,
                Description = x.Description,
                OwnerId = x.CreatedById,
                CreatedAt = x.CreatedDate
            }).FirstOrDefaultAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var project = await context.Projects.FindAsync(id);

        if (project is null)
            throw new Exception("Project nit found");
        
        context.Projects.Remove(project);

        await context.SaveChangesAsync();
    }
}
