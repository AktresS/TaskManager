
using BaseLibrary.Enums;


namespace TaskManager.Services.ProjectAuthorization;

public interface IProjectAuthorizationService
{
    Task<MemberRole> GetUserRole(int projectId);
    Task EnsureMember(int projectId);
    Task EnsureAdmin(int projectId);
    Task EnsureOwner(int projectId);
}
