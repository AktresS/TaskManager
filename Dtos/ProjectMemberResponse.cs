
using TaskManager.Enums;

namespace TaskManager.Dtos;

public class ProjectMemberResponse
{
    public int ProjectMemberId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public MemberRole MemberRole { get; set; }
}
