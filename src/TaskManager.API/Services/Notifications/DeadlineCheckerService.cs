using BaseLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Services.Notifications;

public class DeadlineCheckerService(IServiceScopeFactory scopeFactory, ILogger<DeadlineCheckerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckDeadlinesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Deadline checker error");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CheckDeadlinesAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

        var now = DateTime.UtcNow;
        var in24h = now.AddHours(24);
        var in48h = now.AddHours(48);

        var tasks = await context.WorkItems
            .Include(x => x.Assignees)
            .Where(x =>
                x.DeadLine > now &&
                x.DeadLine <= in48h &&
                x.State != TaskState.Completed)
            .ToListAsync();

        foreach (var task in tasks)
        {
            var link = task.ProjectId.HasValue
                ? $"/projects/{task.ProjectId.Value}/board"
                : "/my-tasks";

            var is24h = task.DeadLine <= in24h;
            var notifLink = is24h ? $"{link}?remind=24" : $"{link}?remind=48";
            var notifText = is24h
                ? $"Дедлайн задачи «{task.Title}» истекает через 24 часа"
                : $"Дедлайн задачи «{task.Title}» истекает через 48 часов";

            if (task.CreatedById.HasValue)
            {
                var alreadyNotified = await context.Notifications.AnyAsync(n =>
                    n.UserId == task.CreatedById.Value &&
                    n.Type == NotificationType.DeadlineReminder &&
                    n.Link == notifLink);

                if (!alreadyNotified)
                {
                    await sender.SendAsync(
                        task.CreatedById.Value,
                        notifText,
                        NotificationType.DeadlineReminder,
                        notifLink);
                }
            }

            foreach (var assignee in task.Assignees)
            {
                if (assignee.UserId == task.CreatedById) continue;

                var assigneeNotified = await context.Notifications.AnyAsync(n =>
                    n.UserId == assignee.UserId &&
                    n.Type == NotificationType.DeadlineReminder &&
                    n.Link == notifLink);

                if (!assigneeNotified)
                {
                    await sender.SendAsync(
                        assignee.UserId,
                        notifText,
                        NotificationType.DeadlineReminder,
                        notifLink);
                }
            }
        }
    }
}