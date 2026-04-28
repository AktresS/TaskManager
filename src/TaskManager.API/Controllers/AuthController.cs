using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Dtos.Auth;
using TaskManager.Services.Auth;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest registerRequest)
        {
            var user = await authService.RegisterAsync(registerRequest);

            if (user is null)
                return BadRequest("Email or name already exists");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest loginRequest)
        {
            var user = await authService.LoginAsync(loginRequest);
            if (user is null)
                return BadRequest("Invalid email or password");

            return Ok(user);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
        {
            var result = await authService.RefreshAsync(request);

            return Ok(result);
        }
    }
}
