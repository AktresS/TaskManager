using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Dtos.WorkItem;
using TaskManager.Services.WorkItems;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/workitems")]
    [ApiController]
    public class WorkItemController(IWorkItemService service) : ControllerBase
    {
        [HttpPost("personal")]
        public async Task<ActionResult<WorkItemResponse>> CreatePersonalTask(CreatePersonalWorkItemRequest request)
        {
            var task = await service.CreatePersonalAsync(request);

            return Ok(task);
        }

        [HttpPost("projects/{projectId}")]
        public async Task<ActionResult<WorkItemResponse>> CreateProjectTask(int projectId, CreateProjectWorkItemRequest request)
        {
            var task = await service.CreateProjectAsync(projectId, request);

            return Ok(task);
        }

        [HttpGet]
        public async Task<ActionResult<WorkItemResponse>> GetMyTask()
        {
            var task = await service.GetMyItemAsync();

            return Ok(task);
        }

        [HttpGet("{workId}")]
        public async Task <ActionResult<WorkItemResponse>> GetById(int workId)
        {
            var task = await service.GetByIdAsync(workId);

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet("projects/{projectId}")]
        public async Task<ActionResult<WorkItemResponse>> GetProjectTasks(int projectId)
        {
            var task = await service.GetProjectTasksAsync(projectId);

            return Ok(task);
        }

        [HttpPatch("{workId}")]
        public async Task<ActionResult<WorkItemResponse>> Update(int workId, UpdateWorkItemRequest request)
        {
            var task = await service.UpdateAsync(workId, request);

            return Ok(task);
        }

        [HttpDelete("{workId}")]
        public async Task<IActionResult> Delete(int workId)
        {
            await service.DeleteAsync(workId);

            return NoContent();
        }
    }
    
}
