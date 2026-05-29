using BaseLibrary.DTOs.WorkItemMessageDtos;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.MessageReads;
using TaskManager.Services.WorkItemMessages;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/workitems/{workItemId}/messages")]
    [ApiController]
    public class WorkItemMessageController(IWorkItemMessageService service) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<WorkItemMessageResponse>> Create(int workItemId, CreateWorkItemMessageRequest request)
        {
            var result = await service.CreateAsync(workItemId, request);

            return Ok(result);
        }

        [HttpPost("read")]
        public async Task<IActionResult> MarkRead(int workitemId, [FromServices] IMessageReadService readService)
        {
            await readService.MarkWorkItemMessagesReadAsync(workitemId);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<WorkItemMessageResponse>> Get(int workItemId)
        {
            var result = await service.GetAsync(workItemId);

            return Ok(result);
        }

        [HttpPatch("{messageId}")]
        public async Task<ActionResult<WorkItemMessageResponse>> Update(int workItemId, int messageId, UpdateWorkItemMessageRequest request)
        {
            var result = await service.UpdateAsync(workItemId, messageId, request);

            return Ok(result);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> Delete(int workItemId, int messageId)
        {
            await service.Delete(workItemId, messageId);

            return NoContent();
        }

    }
}
