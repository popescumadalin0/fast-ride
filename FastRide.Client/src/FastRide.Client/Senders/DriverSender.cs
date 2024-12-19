using System;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastRide.Client.Senders;

public class DriverSender(HubConnection connection) : Sender(connection)
{
    public override UserType UserType => UserType.Driver;
}