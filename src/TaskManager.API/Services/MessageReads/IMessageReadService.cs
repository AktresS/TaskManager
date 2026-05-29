namespace TaskManager.Services.MessageReads;

public interface IMessageReadService
{
    Task MarkProjectMessagesReadAsync(int projectId);
    Task MarkWorkItemMessagesReadAsync(int workItemId);
}
