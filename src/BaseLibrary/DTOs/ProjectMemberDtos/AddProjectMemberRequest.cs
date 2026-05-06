
using BaseLibrary.Enums;

namespace BaseLibrary.DTOs.ProjectMemberDtos;

public class AddProjectMemberRequest
{
    public int UserId { get; set; }
    public MemberRole Role { get; set; }
}
