using BaseLibrary.DTOs.BoardDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Boards;

namespace TaskManager.Controllers
{
    [Route("api/projects/{projectId}/board")]
    [ApiController]
    [Authorize]
    public class BoardController(IBoardService boardService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetByProjectAsync(int projectId)
        {
            var result = await boardService.GetByProjectAsync(projectId);

            return Ok(result);
        } 

        [HttpPost]
        public async Task<IActionResult> CreateAsync(int projectId, CreateBoardRequest request)
        {
            var result = await boardService.CreateAsync(projectId, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await boardService.DeleteAsync(id);

            return NoContent();
        } 
    }
}
