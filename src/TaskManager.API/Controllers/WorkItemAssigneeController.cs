using BaseLibrary.DTOs.AssigneeDtos;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.WorkItemAssignees;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/workitems/{workitemId}/assignees")]
    [ApiController]
    public class WorkItemAssigneeController(IWorkItemAssigneeService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<WorkItemAssigneeResponse>> Get(int workitemId)
        {
            var result = await service.GetAsync(workitemId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int workitemId, AddAssigneeRequest request)
        {
            await service.AddAsync(workitemId, request);

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Remove(int workitemId, int userId)
        {
            await service.RemoveAsync(workitemId, userId);

            return NoContent();
        }
    }
}
