using BaseLibrary.DTOs.TaskDtos;
using BaseLibrary.Enums;
using TaskManager.Data;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.ProjectAuthorization;
using TaskManager.Services.WorkItems;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests;

public class WorkItemServiceTests
{
    private static WorkItemService BuildService(AppDbContext context, int currentUserId, FakeNotificationSender? sender = null)
    {
        ICurrentUserService currentUser = new FakeCurrentUserService(currentUserId);
        var auth = new ProjectAuthorizationService(context, currentUser);
        return new WorkItemService(context, currentUser, auth, sender ?? new FakeNotificationSender());
    }

    [Fact]
    public async Task MoveAsync_ToInProgressColumn_SetsStateAndStartDate()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddProjectTask(context, Seed.MemberUserId, Seed.ColumnTodoId);
        var service = BuildService(context, Seed.MemberUserId);

        await service.MoveAsync(task.WorkItemId, new MoveWorkItemRequest { ColumnId = Seed.ColumnInProgressId });

        var moved = await context.WorkItems.FindAsync(task.WorkItemId);
        Assert.Equal(Seed.ColumnInProgressId, moved!.ColumnId);
        Assert.Equal(TaskState.InProgress, moved.State);
        Assert.NotNull(moved.StartDate);
    }

    [Fact]
    public async Task MoveAsync_ToDoneColumn_SetsCompletedState()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddProjectTask(context, Seed.MemberUserId, Seed.ColumnTodoId);
        var service = BuildService(context, Seed.MemberUserId);

        await service.MoveAsync(task.WorkItemId, new MoveWorkItemRequest { ColumnId = Seed.ColumnDoneId });

        var moved = await context.WorkItems.FindAsync(task.WorkItemId);
        Assert.Equal(TaskState.Completed, moved!.State);
        Assert.NotNull(moved.CompletedAt);
    }

    [Fact]
    public async Task MoveAsync_ToColumnWithoutAutoStatus_KeepsState()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddProjectTask(context, Seed.MemberUserId, Seed.ColumnInProgressId, TaskState.OnHold);
        var service = BuildService(context, Seed.MemberUserId);

        await service.MoveAsync(task.WorkItemId, new MoveWorkItemRequest { ColumnId = Seed.ColumnTodoId });

        var moved = await context.WorkItems.FindAsync(task.WorkItemId);
        Assert.Equal(Seed.ColumnTodoId, moved!.ColumnId);
        Assert.Equal(TaskState.OnHold, moved.State);
    }

    [Fact]
    public async Task MoveAsync_ByNonMember_ThrowsUnauthorized()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddProjectTask(context, Seed.MemberUserId, Seed.ColumnTodoId);
        var service = BuildService(context, Seed.OutsiderUserId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.MoveAsync(task.WorkItemId, new MoveWorkItemRequest { ColumnId = Seed.ColumnInProgressId }));
    }

    [Fact]
    public async Task CreateProjectAsync_WithoutAssignees_AssignsCreator()
    {
        using var context = TestDb.CreateContext();
        var service = BuildService(context, Seed.MemberUserId);

        var request = new CreateProjectWorkItemRequest
        {
            Title = "Новая задача",
            DeadLine = DateTime.UtcNow.AddDays(2),
            ColumnId = Seed.ColumnTodoId,
            AssigneeIds = new List<int>()
        };

        var result = await service.CreateProjectAsync(Seed.ProjectId, Seed.ColumnTodoId, request);

        Assert.Single(result.Assignees);
        Assert.Equal(Seed.MemberUserId, result.Assignees[0].Id);
    }

    [Fact]
    public async Task CreateProjectAsync_ColumnFromOtherProject_Throws()
    {
        using var context = TestDb.CreateContext();
        var service = BuildService(context, Seed.MemberUserId);

        var request = new CreateProjectWorkItemRequest
        {
            Title = "Задача",
            DeadLine = DateTime.UtcNow.AddDays(2),
            ColumnId = Seed.OtherColumnId
        };

        await Assert.ThrowsAsync<Exception>(
            () => service.CreateProjectAsync(Seed.ProjectId, Seed.OtherColumnId, request));
    }

    [Fact]
    public async Task CreateProjectAsync_AssigneeNotMember_Throws()
    {
        using var context = TestDb.CreateContext();
        var service = BuildService(context, Seed.MemberUserId);

        var request = new CreateProjectWorkItemRequest
        {
            Title = "Задача",
            DeadLine = DateTime.UtcNow.AddDays(2),
            ColumnId = Seed.ColumnTodoId,
            AssigneeIds = new List<int> { Seed.OutsiderUserId }
        };

        await Assert.ThrowsAsync<Exception>(
            () => service.CreateProjectAsync(Seed.ProjectId, Seed.ColumnTodoId, request));
    }

    [Fact]
    public async Task UpdateAsync_StateCompletedThenInProgress_TogglesCompletedAt()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddProjectTask(context, Seed.MemberUserId, Seed.ColumnTodoId);
        var service = BuildService(context, Seed.MemberUserId);

        await service.UpdateAsync(task.WorkItemId, new UpdateWorkItemRequest { State = TaskState.Completed });
        var afterComplete = await context.WorkItems.FindAsync(task.WorkItemId);
        Assert.NotNull(afterComplete!.CompletedAt);

        await service.UpdateAsync(task.WorkItemId, new UpdateWorkItemRequest { State = TaskState.InProgress });
        var afterReopen = await context.WorkItems.FindAsync(task.WorkItemId);
        Assert.Null(afterReopen!.CompletedAt);
        Assert.Equal(TaskState.InProgress, afterReopen.State);
    }

    [Fact]
    public async Task UpdateAsync_PersonalTaskByNonOwner_ThrowsUnauthorized()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddPersonalTask(context, Seed.OwnerUserId);
        var service = BuildService(context, Seed.MemberUserId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.UpdateAsync(task.WorkItemId, new UpdateWorkItemRequest { Title = "Чужое" }));
    }

    [Fact]
    public async Task DeleteAsync_ProjectOwnerDeletesMemberTask_Succeeds()
    {
        using var context = TestDb.CreateContext();
        var task = TestDb.AddProjectTask(context, Seed.MemberUserId, Seed.ColumnTodoId);
        var service = BuildService(context, Seed.OwnerUserId);

        await service.DeleteAsync(task.WorkItemId);

        Assert.Null(await context.WorkItems.FindAsync(task.WorkItemId));
    }
}