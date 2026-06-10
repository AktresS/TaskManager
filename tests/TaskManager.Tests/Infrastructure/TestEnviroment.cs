using BaseLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.Notifications;

namespace TaskManager.Tests.Infrastructure;

public class FakeCurrentUserService(int userId, string userName = "test") : ICurrentUserService
{
    public int UserId { get; } = userId;
    public string UserName { get; } = userName;
}

public class FakeNotificationSender : INotificationSender
{
    public int SentCount { get; private set; }

    public Task SendAsync(int userId, string text, NotificationType type, string? link = null)
    {
        SentCount++;
        return Task.CompletedTask;
    }
}

public static class Seed
{
    public const int OwnerUserId = 1;
    public const int MemberUserId = 2;
    public const int OutsiderUserId = 3;
    public const int AdminUserId = 4;

    public const int ProjectId = 10;
    public const int OtherProjectId = 11;

    public const int ColumnTodoId = 31;
    public const int ColumnInProgressId = 32; 
    public const int ColumnDoneId = 33;
    public const int OtherColumnId = 41;
}

public static class TestDb
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"taskmanager-tests-{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
        Populate(context);
        return context;
    }

    private static void Populate(AppDbContext context)
    {
        context.Users.AddRange(
            new User { UserId = Seed.OwnerUserId, Name = "Никита", Email = "owner@test.local", PasswordHash = "x" },
            new User { UserId = Seed.MemberUserId, Name = "Борис", Email = "member@test.local", PasswordHash = "x" },
            new User { UserId = Seed.OutsiderUserId, Name = "Виктор", Email = "outsider@test.local", PasswordHash = "x" },
            new User { UserId = Seed.AdminUserId, Name = "Галина", Email = "admin@test.local", PasswordHash = "x" });

        context.Projects.AddRange(
            new Project { ProjectId = Seed.ProjectId, Name = "Демо-проект", CreatedById = Seed.OwnerUserId },
            new Project { ProjectId = Seed.OtherProjectId, Name = "Другой проект", CreatedById = Seed.OwnerUserId });

        context.ProjectMembers.AddRange(
            new ProjectMember { ProjectId = Seed.ProjectId, UserId = Seed.OwnerUserId, Role = MemberRole.Owner },
            new ProjectMember { ProjectId = Seed.ProjectId, UserId = Seed.MemberUserId, Role = MemberRole.Member },
            new ProjectMember { ProjectId = Seed.ProjectId, UserId = Seed.AdminUserId, Role = MemberRole.Admin },
            new ProjectMember { ProjectId = Seed.OtherProjectId, UserId = Seed.OwnerUserId, Role = MemberRole.Owner });

        var board = new Board { Id = 20, ProjectId = Seed.ProjectId, Name = "Основная доска" };
        var otherBoard = new Board { Id = 21, ProjectId = Seed.OtherProjectId, Name = "Доска другого проекта" };
        context.Boards.AddRange(board, otherBoard);

        context.BoardColumns.AddRange(
            new BoardColumn { Id = Seed.ColumnTodoId, BoardId = 20, Name = "К выполнению", Order = 0, AutoStatus = null },
            new BoardColumn { Id = Seed.ColumnInProgressId, BoardId = 20, Name = "В работе", Order = 1, AutoStatus = TaskState.InProgress },
            new BoardColumn { Id = Seed.ColumnDoneId, BoardId = 20, Name = "Готово", Order = 2, AutoStatus = TaskState.Completed },
            new BoardColumn { Id = Seed.OtherColumnId, BoardId = 21, Name = "Чужая колонка", Order = 0, AutoStatus = null });

        context.SaveChanges();
    }

    public static WorkItem AddProjectTask(AppDbContext context, int createdById, int columnId, TaskState state = TaskState.New)
    {
        var item = new WorkItem
        {
            Title = "Задача",
            ProjectId = Seed.ProjectId,
            ColumnId = columnId,
            CreatedById = createdById,
            State = state,
            DeadLine = DateTime.UtcNow.AddDays(3)
        };
        context.WorkItems.Add(item);
        context.SaveChanges();
        return item;
    }

    public static WorkItem AddPersonalTask(AppDbContext context, int createdById)
    {
        var item = new WorkItem
        {
            Title = "Личная задача",
            ProjectId = null,
            ColumnId = null,
            CreatedById = createdById,
            State = TaskState.New,
            DeadLine = DateTime.UtcNow.AddDays(3)
        };
        context.WorkItems.Add(item);
        context.SaveChanges();
        return item;
    }
}