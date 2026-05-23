using BaseLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Services.CurrentUser;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController(AppDbContext context, ICurrentUserService currentUser) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = currentUser.UserId;

            var notifications = await context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .Select(x => new NotificationResponse
                {
                    NotificationId = x.NotificationId,
                    Text = x.Text,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    Link = x.Link,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = currentUser.UserId;

            var notification = await context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.UserId == userId);

            if (notification is null) return NotFound();

            notification.IsRead = true;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = currentUser.UserId;

            await context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ExecuteUpdateAsync(x => x.SetProperty(n => n.IsRead, true));

            return NoContent();
        }

        [HttpDelete("read")]
        public async Task<IActionResult> DeleteRead()
        {
            var userId = currentUser.UserId;

            await context.Notifications
                .Where(x => x.UserId == userId && x.IsRead)
                .ExecuteDeleteAsync();

            return NoContent();
        }
    }
}
