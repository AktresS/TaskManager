using BaseLibrary.Responses;

namespace TaskManager.Services.ChatRealtimeNotifier;

public interface IChatRealtimeNotifier
{
    Task ProjectMessageAsync(IEnumerable<int> userIds, ProjectMessageResponse message);
    Task WorkItemMessageAsync(IEnumerable<int> userIds, WorkItemMessageResponse message);
}
