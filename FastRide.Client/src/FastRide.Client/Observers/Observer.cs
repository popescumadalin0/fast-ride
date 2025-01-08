using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Constants;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Observers;

public class Observer : IObserver
{
    private readonly HubConnection _connection;

    public Observer(HubConnection connection)
    {
        _connection = connection;

        _connection.On(SignalRConstants.NotifyUserGeolocation,
            async (string userId, Server.Contracts.Models.Geolocation message) => await OnNotifyDriverGeolocationAsync(
                userId, new Geolocation()
                {
                    Latitude = message.Latitude,
                    Longitude = message.Longitude
                }));
    }

    /// <inheritdoc />
    public event Func<string, Geolocation, Task> NotifyDriverGeolocation = null!;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    private async Task OnNotifyDriverGeolocationAsync(string userId, Geolocation arg)
    {
        if (NotifyDriverGeolocation != null!) await NotifyDriverGeolocation(userId, arg);
    }
}