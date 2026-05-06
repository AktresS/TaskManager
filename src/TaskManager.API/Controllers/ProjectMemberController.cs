using BaseLibrary.DTOs.ProjectMemberDtos;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPut("{userId}")]
        public async Task<ActionResult<ProjectMemberRoleResponse>> UpdateMemberRole(int projectId, int userId, UpdateProjectRoleRequest request)
        {
            var response = await service.UpdateProjectMemberRole(projectId, userId, request);
            return Ok(response);
        }    


        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveMember(int projectId, int userId)
        {
            await service.RemoveMemberAsync(projectId, userId);

            return NoContent();
        }
    }
}
