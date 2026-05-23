using BaseLibrary.DTOs.ProjectDtos;
using BaseLibrary.Enums;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.Projects;

public class ProjectService(AppDbContext context, ICurrentUserService currentUser, IProjectAuthorizationService auth) : IProjectService
{
    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty");

        if (name.Length > 256)
            throw new ArgumentException("Project name is too long");
            
        var project = new Project
        {
            Name = name,
            Description = request.Description,
            CreatedById = currentUser.UserId,
            CreatedDate = DateTime.UtcNow
        };

        context.Projects.Add(project);

        context.ProjectMembers.Add(new ProjectMember
        {
            Project = project,
            UserId = currentUser.UserId,
            Role = MemberRole.Owner
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

        await auth.EnsureOwner(id);
        
        if (request.Name != null)
            project.Name = request.Name;

        if (request.Description != null)
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
        var userId = currentUser.UserId;
        var pinned = await context.PinnedProjects
            .Where(x => x.UserId == userId)
            .Select(x => x.ProjectId)
            .ToHashSetAsync();
        
        return await context.ProjectMembers
            .Where(x => x.UserId == userId)
            .Select(x => new ProjectResponse
            {
                ProjectId = x.ProjectId,
                Name = x.Project.Name, 
                Description = x.Project.Description,
                OwnerId = x.Project.CreatedById,
                CreatedAt = x.Project.CreatedDate,
                IsPinned = pinned.Contains(x.ProjectId)
            }).ToListAsync();
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        var userId = currentUser.UserId;

        var isPinned = await context.PinnedProjects
            .AnyAsync(x => x.UserId == userId && x.ProjectId == id);

        return await context.Projects
            .Where(x => x.ProjectId == id)
            .Select(x => new ProjectResponse
            {
                ProjectId = x.ProjectId,
                Name = x.Name,
                Description = x.Description,
                OwnerId = x.CreatedById,
                CreatedAt = x.CreatedDate,
                IsPinned = isPinned
            }).FirstOrDefaultAsync();
    }
    
    public async Task<bool> TogglePinAsync(int projectId)
    {
        var userId = currentUser.UserId;

        var existing = await context.PinnedProjects
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProjectId == projectId);

        if (existing is not null)
        {
            context.PinnedProjects.Remove(existing);
            await context.SaveChangesAsync();
            return false;
        }

        context.PinnedProjects.Add(new UserPinnedProject
        {
            UserId = userId,
            ProjectId = projectId
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(int id)
    {
        var project = await context.Projects.FindAsync(id);

        if (project is null)
            throw new Exception("Project not found");

        await auth.EnsureOwner(id);
        
        context.Projects.Remove(project);

        await context.SaveChangesAsync();
    }
}
