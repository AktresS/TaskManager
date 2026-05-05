
using BaseLibrary.Enums;

namespace TaskManager.Dtos;

public class AddProjectMemberRequest
{
    public int UserId { get; set; }
    public MemberRole Role { get; set; }
}
