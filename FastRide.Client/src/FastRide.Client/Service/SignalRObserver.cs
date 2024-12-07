﻿using System;
using System.Threading.Tasks;
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
        _connection.On("AvailableRides", async (Ride message) => await OnAvailableRidesUpdatedAsync(message));
    }

    /// <inheritdoc />
    public event Func<Ride, Task> AvailableRiders = default!;

    /// <inheritdoc />
    public event Func<Ride, Task> AvailableRides = default!;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    private async Task OnAvailableRidesUpdatedAsync(Ride arg)
    {
        if (AvailableRides != null!) await AvailableRides(arg);
    }

    private async Task OnAvailableRidersUpdatedAsync(Ride arg)
    {
        if (AvailableRiders != null!) await AvailableRiders(arg);
    }
}