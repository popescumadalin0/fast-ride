using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Observers;

public class Observer : IObserver
{
    private readonly HubConnection _connection;

    public Observer(HubConnection connection)
    {
        _connection = connection;

        _connection.On(SignalRConstants.NotifyUserGeolocation,
            async (string userId, Geolocation message) => await OnNotifyUserGeolocationAsync(userId, message));
    }

    /// <inheritdoc />
    public event Func<string, Geolocation, Task> NotifyUserGeolocation = default!;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    private async Task OnNotifyUserGeolocationAsync(string userId, Geolocation arg)
    {
        if (NotifyUserGeolocation != null!) await NotifyUserGeolocation(userId, arg);
    }
}