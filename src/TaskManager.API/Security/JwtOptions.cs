using System;

namespace TaskManager.Security;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpireHours { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
