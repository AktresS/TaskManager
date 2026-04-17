using System;

namespace TaskManager.Services.CurrentUser;

public interface ICurrentUserService
{
    public int UserId { get; }
    public string UserName { get; }
}
