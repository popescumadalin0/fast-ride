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
    private readonly IConfiguration _configuration;
    private HubConnection _connection;

    public SignalRService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async ValueTask StartConnectionAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{_configuration["FastRide:BaseUrl"]!}/api")
            .WithAutomaticReconnect()
            .Build();
        
        _connection.Closed += async (_) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await Connect();
        };
        
        await Connect();
    }

    public ValueTask InitiateSignalRSubscribersAsync()
    {
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
        return ValueTask.CompletedTask;
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

    public Task AcceptRideAsync(RideInformation rideInformation)
    {
        return Task.CompletedTask;
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
    
    private async Task Connect()
    {
        try
        {
            if (_connection != null)
            {
                    
                Console.WriteLine("Trying to connect");
                await _connection.StartAsync();
                _connection.Reconnecting += error =>
                {
                    Console.WriteLine($"Reconnecting... : {error} ");
                    return Task.CompletedTask;
                };

                _connection.Reconnecting += error =>
                {
                    Console.WriteLine($"Reconnected... : {error} ");
                    return Task.CompletedTask;
                };

                _connection.Closed += error =>
                {
                    Console.WriteLine($"Connection Closed... : {error} ");
                    return Task.CompletedTask;
                };
                Console.WriteLine($"Connected: {_connection.ConnectionId} {_connection.State}");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAILED to connect SignalR {ex.Message}");
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }
}