using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace FastRide.Client.Service;

public class SignalRService : ISignalRService
{
    private readonly IConfiguration _configuration;
    private HubConnection _connection;

    public SignalRService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public event Func<PriceCalculated, Task> SendPriceCalculated;

    /// <inheritdoc />
    public event Func<SendPaymentIntent, Task> SendPaymentIntentReceived;

    /// <inheritdoc />
    public event Func<NotifyUserGeolocation, Task> NotifyDriverGeolocation = null!;

    /// <inheritdoc />
    public event Func<RideCreated, Task> RideCreated = null!;

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
        _connection.On<NotifyUserGeolocation>(SignalRConstants.ServerNotifyUserGeolocation,
            async (payload) =>
            {
                if (NotifyDriverGeolocation != null!)
                {
                    await NotifyDriverGeolocation(payload);
                }
            });

        _connection.On<RideCreated>(SignalRConstants.ServerCreateNewRide,
            async (payload) =>
            {
                if (RideCreated != null!)
                {
                    await RideCreated(payload);
                }
            });

        _connection.On<PriceCalculated>(SignalRConstants.ServerSendPriceCalculation,
            async (payload) =>
            {
                if (SendPriceCalculated != null!)
                {
                    await SendPriceCalculated(payload);
                }
            });

        _connection.On<SendPaymentIntent>(SignalRConstants.ServerSendPaymentIntent,
            async (payload) =>
            {
                if (SendPaymentIntentReceived != null!)
                {
                    await SendPaymentIntentReceived(payload);
                }
            });
        return ValueTask.CompletedTask;
    }

    public async Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation)
    {
        await _connection.SendAsync(SignalRConstants.ClientNotifyUserGeolocation, userId, groupName,
            new Geolocation()
            {
                Latitude = geolocation.Latitude,
                Longitude = geolocation.Longitude
            });
    }

    public async Task AcceptRideAsync(RideInformation rideInformation)
    {
        //todo
        await _connection.SendAsync(SignalRConstants.ClientAcceptRide);
    }

    public async Task CreateNewRideAsync(string groupName, NewRideInput rideInput)
    {
        await _connection.SendAsync(SignalRConstants.ClientCreateNewRide, groupName, rideInput);
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.ClientLeaveUserFromGroup, userId, groupName);
    }

    public async Task ConfirmPriceCalculatedAsync(string instanceId, decimal priceConfirmed)
    {
        await _connection.SendAsync(SignalRConstants.ClientSendPriceCalculation, instanceId, priceConfirmed);
    }

    public async Task ConfirmPaymentAsync(string instanceId, bool paymentSuccess)
    {
        await _connection.SendAsync(SignalRConstants.ClientSendPaymentIntent, instanceId, paymentSuccess);
    }

    public async Task JoinUserInGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.ClientJoinUserToGroup, userId, groupName);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
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
}