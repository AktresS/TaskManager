using System;
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
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            Console.WriteLine($"[CurrentUserService] Claim value: '{userIdClaim}'");

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Невозможно определить пользователя из токена");
            }

            return userId;
        }
    }

    int ICurrentUserService.UserId { get => UserId; set => throw new NotImplementedException(); }
}
