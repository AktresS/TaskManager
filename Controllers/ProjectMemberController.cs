using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Dtos;
using TaskManager.Services.ProjectMembers;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("projects/{projectId}/members")]
    [ApiController]
    public class ProjectMemberController(IProjectMemberService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMembers(int projectId)
        {
            var members = await service.GetMembersAsync(projectId);
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int projectId, AddProjectMemberRequest request)
        {
            await service.AddMemberAsync(projectId, request);
            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveMember(int projectId, int userId)
        {
            await service.RemoveMemberAsync(projectId, userId);

            return Ok();
        }
    }
}
