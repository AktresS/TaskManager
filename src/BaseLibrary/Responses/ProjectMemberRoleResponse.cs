using BaseLibrary.Enums;

namespace BaseLibrary.Responses;

public class ProjectMemberRoleResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public MemberRole Role { get; set; }
}
