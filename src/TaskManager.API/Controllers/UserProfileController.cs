using BaseLibrary.DTOs.SettingsDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.UserProfile;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController(IUserProfileService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await service.GetProfileAsync();
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            await service.ChangePasswordAsync(request);
            return NoContent();
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(UpdateUserSettingsRequest request)
        {
            await service.UpdateSettingsAsync(request);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            await service.DeleteAccountAsync();
            return NoContent();
        }
    }
}
