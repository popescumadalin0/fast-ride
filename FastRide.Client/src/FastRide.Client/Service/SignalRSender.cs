using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Service;

public class SignalRSender : ISignalRSender
{
    private readonly HubConnection _connection;

    /// A wrapper around a SignalR connection that receives notifications about rides.
    public SignalRSender(HubConnection connection)
    {
        _connection = connection;

        _connection.StartAsync().GetAwaiter().GetResult();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    public async Task AcceptRideAsync(Ride ride)
    {
        await _connection.SendAsync("AcceptRide", ride);
    }

    public async Task BookRideAsync(Ride ride)
    {
        await _connection.SendAsync("BookRide", ride);
    }
}