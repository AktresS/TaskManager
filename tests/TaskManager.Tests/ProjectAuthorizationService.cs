using BaseLibrary.Enums;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests;

public class ProjectAuthorizationServiceTests
{
    private static ProjectAuthorizationService BuildService(int currentUserId)
    {
        var context = TestDb.CreateContext();
        ICurrentUserService currentUser = new FakeCurrentUserService(currentUserId);
        return new ProjectAuthorizationService(context, currentUser);
    }

    [Fact]
    public async Task GetUserRole_ReturnsRoleForMember()
    {
        var service = BuildService(Seed.MemberUserId);

        var role = await service.GetUserRole(Seed.ProjectId);

        Assert.Equal(MemberRole.Member, role);
    }

    [Fact]
    public async Task GetUserRole_NonMember_ThrowsUnauthorized()
    {
        var service = BuildService(Seed.OutsiderUserId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetUserRole(Seed.ProjectId));
    }

    [Fact]
    public async Task EnsureAdmin_Member_ThrowsUnauthorized()
    {
        var service = BuildService(Seed.MemberUserId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.EnsureAdmin(Seed.ProjectId));
    }

    [Fact]
    public async Task EnsureAdmin_Admin_DoesNotThrow()
    {
        var service = BuildService(Seed.AdminUserId);

        await service.EnsureAdmin(Seed.ProjectId);
    }

    [Fact]
    public async Task EnsureOwner_Admin_ThrowsUnauthorized()
    {
        var service = BuildService(Seed.AdminUserId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.EnsureOwner(Seed.ProjectId));
    }

    [Fact]
    public async Task EnsureOwner_Owner_DoesNotThrow()
    {
        var service = BuildService(Seed.OwnerUserId);

        await service.EnsureOwner(Seed.ProjectId);
    }
}