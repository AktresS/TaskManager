using BaseLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController(AppDbContext context) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Ok(new List<object>());

            var users = await context.Users
                .Where(x => x.Name.Contains(q) || x.Email.Contains(q))
                .Select(x => new UserSearchResponse
                {
                    Id    = x.UserId,
                    Name  = x.Name,
                    Email = x.Email
                })
                .Take(10)
                .ToListAsync();

            return Ok(users);
        }
    }
}
