using BaseLibrary.DTOs.ProjectDtos;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Projects;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/projects")]
    [ApiController]
    public class ProjectController(IProjectService projectService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> Create(CreateProjectRequest request)
        {
            var project = await projectService.CreateAsync(request);

            return Ok(project);
        }

        [HttpGet]
        public async Task<ActionResult<ProjectResponse>> GetProjects()
        {
            var project = await projectService.GetUserProjectsAsync();

            return Ok(project);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetProject(int id)
        {
            var project = await projectService.GetByIdAsync(id);

            if (project is null)
                return NotFound();

            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectResponse>> Update(int id, UpdateProjectRequest request)
        {
            var project = await projectService.UpdateAsync(id, request);

            return Ok(project);
        }

        [HttpPost("{id}/pin")]
        public async Task<ActionResult<bool>> TogglePin(int id)
        {
            var isPinned = await projectService.TogglePinAsync(id);
            return Ok(isPinned);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await projectService.DeleteAsync(id);

            return NoContent();
        }
    }
}
