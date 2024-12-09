using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Service;

public class SignalRObserver : ISignalRObserver
{
    private readonly HubConnection _connection;

    /// A wrapper around a SignalR connection that receives notifications about rides.
    public SignalRObserver(HubConnection connection)
    {
        _connection = connection;

        _connection.On("AvailableRiders", async (Ride message) => await OnAvailableRidersUpdatedAsync(message));
        _connection.On("AvailableRide", async (Ride message) => await OnAvailableRideUpdatedAsync(message));
        
        _connection.StartAsync().GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public event Func<Ride, Task> AvailableRiders = default!;

    /// <inheritdoc />
    public event Func<Ride, Task> AvailableRide = default!;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    private async Task OnAvailableRideUpdatedAsync(Ride arg)
    {
        if (AvailableRide != null!) await AvailableRide(arg);
    }

    private async Task OnAvailableRidersUpdatedAsync(Ride arg)
    {
        if (AvailableRiders != null!) await AvailableRiders(arg);
    }
}