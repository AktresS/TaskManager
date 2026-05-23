using System.Net.Http.Json;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace ClientLibrary.Services.Implementations;

public class NotificationService : IAsyncDisposable
{
    private readonly GetHttpClient _getHttpClient;
    private readonly string _apiBaseUrl;
    private HubConnection? _hub;

    public NotificationService(GetHttpClient getHttpClient, IConfiguration configuration)
    {
        _getHttpClient = getHttpClient;
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"]!;
    }

    public List<NotificationResponse> Notifications { get; private set; } = new();
    public int UnreadCount => Notifications.Count(x => !x.IsRead);

    public event Action? OnChanged;

    public async Task InitAsync(string token)
    {
        // Загружаем существующие уведомления
        var client = await _getHttpClient.GetPrivateHttpClient();
        var result = await client.GetFromJsonAsync<List<NotificationResponse>>("api/notifications");
        Notifications = result ?? new();

        _hub = new HubConnectionBuilder()
            .WithUrl($"{_apiBaseUrl}/hubs/notifications", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _hub.On<NotificationResponse>("ReceiveNotification", notification =>
        {
            Notifications.Insert(0, notification);
            OnChanged?.Invoke();
        });

        await _hub.StartAsync();
    }

    public async Task MarkReadAsync(int id)
    {
        var client = await _getHttpClient.GetPrivateHttpClient();
        await client.PostAsync($"api/notifications/{id}/read", null);

        var n = Notifications.FirstOrDefault(x => x.NotificationId == id);
        if (n is not null) n.IsRead = true;

        OnChanged?.Invoke();
    }

    public async Task MarkAllReadAsync()
    {
        var client = await _getHttpClient.GetPrivateHttpClient();
        await client.PostAsync("api/notifications/read-all", null);

        Notifications.ForEach(x => x.IsRead = true);
        OnChanged?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub is not null)
            await _hub.DisposeAsync();
    }

    public async Task DeleteReadAsync()
    {
        var client = await _getHttpClient.GetPrivateHttpClient();
        await client.DeleteAsync("api/notifications/read");

        Notifications.RemoveAll(x => x.IsRead);
        OnChanged?.Invoke();
    }
}
