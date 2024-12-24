using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Geolocation = FastRide.Server.Contracts.Models.Geolocation;

namespace FastRide.Client.Senders;

public abstract class Sender(HubConnection connection) : ISender
{
    protected readonly HubConnection Connection = connection;

    public abstract UserType UserType { get; }

    public async Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation)
    {
        await Connection.SendAsync(SignalRConstants.NotifyUserGeolocation, userId, groupName, geolocation);
    }

    public async Task JoinUserInGroupAsync(string userId, string groupName)
    {
        await Connection.SendAsync(SignalRConstants.JoinUserToGroup, userId, groupName);
    }

    public async ValueTask DisposeAsync()
    {
        await Connection.StopAsync();
        await Connection.DisposeAsync();
    }
}