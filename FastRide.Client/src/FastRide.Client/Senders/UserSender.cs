using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Senders;

public class UserSender(HubConnection connection) : Sender(connection)
{
    public override UserType UserType => UserType.User;
}