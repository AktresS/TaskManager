using TaskManager.Enums;

namespace TaskManager.Dtos;

public class ProjectMemberRoleResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public MemberRole Role { get; set; }
}
