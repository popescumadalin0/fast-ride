using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Observers;

public class DriverObserver : IObserver
{
    private readonly HubConnection _connection;

    public UserType UserType { get; } = UserType.Driver;

    public DriverObserver(HubConnection connection)
    {
        _connection = connection;

        _connection.On("AvailableRiders", async (Ride message) => await OnAvailableRidersUpdatedAsync(message));
        _connection.On("AvailableRide", async (Ride message) => await OnAvailableRideUpdatedAsync(message));
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