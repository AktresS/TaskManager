using BaseLibrary.DTOs.ProjectMessageDtos;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.ProjectMessages;

namespace TaskManager.Controllers
{
    [Route("projects/{projectId}/messages")]
    [ApiController]
    public class ProjectMessageController(IProjectMessageService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMessages(int projectId)
        {
            var result = await service.GetMessagesAsync(projectId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectMessageResponse>> CreateMessage(int projectId, CreateProjectMessageRequest request)
        {
            var result = await service.CreateAsync(projectId, request);

            return Ok(result);
        }

        [HttpPut("{messageId}")]
        public async Task<ActionResult<ProjectMessageResponse>> UpdateMessage(int messageId, int projectId, UpdateProjectMessageRequest request)
        {
            var result = await service.UpdateAsync(projectId, messageId, request);

            return Ok(result);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> RemoveMessage(int projectId, int messageId)
        {
            await service.DeleteAsync(projectId, messageId);

            return NoContent();
        }
    }
}
