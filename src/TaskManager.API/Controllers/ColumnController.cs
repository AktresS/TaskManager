using BaseLibrary.DTOs.ColumnDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Columns;

namespace TaskManager.Controllers
{
    [Route("api/boards/{boardId}/columns")]
    [ApiController]
    [Authorize]
    public class ColumnController(IColumnService columnService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetByBoardAsync(int boardId) => Ok(await columnService.GetByBoardAsync(boardId));

        [HttpPost]
        public async Task<IActionResult> CreateAsync(int boardId, CreateColumnRequest request)
        {
            var result = await columnService.CreateAsync(boardId, request);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateColumnRequest request)
        {
            var result = await columnService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await columnService.DeleteAsync(id);
            return NoContent();
        }
    }
}
