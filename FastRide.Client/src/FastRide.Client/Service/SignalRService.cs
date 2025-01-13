using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Geolocation = FastRide.Client.Models.Geolocation;

namespace FastRide.Client.Service;

public class SignalRService : ISignalRService
{
    private readonly HubConnection _connection;

    public SignalRService(IConfiguration configuration)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{configuration["FastRide:BaseUrl"]!}/api")
            .WithAutomaticReconnect()
            .Build();

        _connection.On(SignalRConstants.NotifyUserGeolocation,
            async (string userId, Server.Contracts.Models.Geolocation message) =>
            {
                if (NotifyDriverGeolocation != null!)
                {
                    await NotifyDriverGeolocation(userId, new Geolocation()
                    {
                        Latitude = message.Latitude,
                        Longitude = message.Longitude
                    });
                }
            });

        _connection.On(SignalRConstants.RideCreated,
            async (string instanceId) =>
            {
                if (RideCreated != null!)
                {
                    await RideCreated(instanceId);
                }
            });
    }

    /// <inheritdoc />
    public event Func<string, Geolocation, Task> NotifyDriverGeolocation = null!;

    /// <inheritdoc />
    public event Func<string, Task> RideCreated = null!;

    public async Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation)
    {
        await _connection.SendAsync(SignalRConstants.NotifyUserGeolocation, userId, groupName,
            new Server.Contracts.Models.Geolocation()
            {
                Latitude = geolocation.Latitude,
                Longitude = geolocation.Longitude
            });
    }

    public async Task AcceptRideAsync(RideInformation rideInformation)
    {
        //await _connection.SendAsync(SignalRConstants.AcceptRide, userId, groupName);
    }

    public async Task CreateNewRideAsync(string groupName, NewRideInput rideInput)
    {
        await _connection.SendAsync(SignalRConstants.CreateNewRide, groupName, rideInput);
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.LeaveUserFromGroup, userId, groupName);
    }

    public async Task JoinUserInGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.JoinUserToGroup, userId, groupName);
    }

    public async ValueTask StartConnectionAsync()
    {
        await _connection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }
}