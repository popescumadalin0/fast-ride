using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Senders;

public abstract class Sender(HubConnection connection) : ISender
{
    protected readonly HubConnection Connection = connection;
    
    public abstract UserType UserType { get; }

    public async ValueTask DisposeAsync()
    {
        await Connection.StopAsync();
        await Connection.DisposeAsync();
    }
    
    public async Task JoinUserInGroupAsync(string userId)
    {
        await Connection.SendAsync("JoinUserToGroup", userId, UserType);
    }
}