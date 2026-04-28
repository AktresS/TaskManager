using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace TaskManager.Dtos.Auth;

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
