using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.Files;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/files")]
    [ApiController]
    public class FileController(IFileService fileService, AppDbContext context, ICurrentUserService currentUser) : ControllerBase
    {
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var url = await fileService.UploadImageAsync(file);

            var user = await context.Users.FindAsync(currentUser.UserId);
            if (user is null) return NotFound();

            user.AvatarUrl = url;
            await context.SaveChangesAsync();

            return Ok(new { url });
        }

        [HttpPost("attachment")]
        public async Task<IActionResult> UploadAttachment(IFormFile file)
        {
            var url = await fileService.UploadFileAsync(file);
            return Ok(new { url });
        }
    }
}
