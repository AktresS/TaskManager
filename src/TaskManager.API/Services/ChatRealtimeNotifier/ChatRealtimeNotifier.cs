using BaseLibrary.Responses;
using Microsoft.AspNetCore.SignalR;
using TaskManager.Hubs;

namespace TaskManager.Services.ChatRealtimeNotifier;

public class ChatRealtimeNotifier(IHubContext<NotificationHub> hub) : IChatRealtimeNotifier
{
    public Task ProjectMessageAsync(IEnumerable<int> userIds, ProjectMessageResponse message)
        => SendAsync(userIds, "ReceiveProjectMessage", message);

    public Task WorkItemMessageAsync(IEnumerable<int> userIds, WorkItemMessageResponse message)
        => SendAsync(userIds, "ReceiveWorkItemMessage", message);

    private async Task SendAsync(IEnumerable<int> userIds, string method, object payload)
    {
        var groups = userIds
            .Distinct()
            .Select(id => $"user_{id}")
            .ToList();

        if (groups.Count == 0) return;

        await hub.Clients.Groups(groups).SendAsync(method, payload);
    }
}
