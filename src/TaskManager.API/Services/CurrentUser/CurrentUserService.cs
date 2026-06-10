using System.Security.Claims;

namespace TaskManager.Services.CurrentUser;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Unable to determine user from token");
            }

            return userId;
        }
    }

    public string UserName
    {
        get
        {
            var userNameClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userNameClaim))
            {
                throw new UnauthorizedAccessException("Unable to determine username from token");
            }

            return userNameClaim;
        }
    }

}
