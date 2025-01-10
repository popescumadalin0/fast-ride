using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using Geolocation = FastRide.Server.Contracts.Models.Geolocation;

namespace FastRide.Client.Senders;

public class Sender(HubConnection connection) : ISender
{
    private readonly HubConnection _connection = connection;

    public async Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation)
    {
        await _connection.SendAsync(SignalRConstants.NotifyUserGeolocation, userId, groupName, geolocation);
    }

    public Task AcceptRideAsync(RideInformation rideInformation)
    {
        throw new System.NotImplementedException();
    }

    public async Task JoinUserInGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.JoinUserToGroup, userId, groupName);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }
}