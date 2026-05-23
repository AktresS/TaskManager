using System;

namespace BaseLibrary.Responses;

public class UserSearchResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
