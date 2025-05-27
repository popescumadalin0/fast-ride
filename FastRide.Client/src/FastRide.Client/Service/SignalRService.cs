using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace FastRide.Client.Service;

public class SignalRService : ISignalRService
{
    private readonly IConfiguration _configuration;

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private HubConnection _connection;

    public SignalRService(IConfiguration configuration, AuthenticationStateProvider authenticationStateProvider)
    {
        _configuration = configuration;
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <inheritdoc />
    public event Func<PriceCalculated, Task> SendPriceCalculated;

    /// <inheritdoc />
    public event Func<SendPaymentIntent, Task> SendPaymentIntentReceived;

    /// <inheritdoc />
    public event Func<DriverAcceptRide, Task> DriverAcceptRide;

    /// <inheritdoc />
    public event Func<RatingRequest, Task> SendRating;

    /// <inheritdoc />
    public event Func<Task> DriverRideAccepted;

    /// <inheritdoc />
    public event Func<Task> CancelRide;

    /// <inheritdoc />
    public event Func<Ride, Task> NotifyState;

    public event Func<Task> NotifyDriverTimeout;

    /// <inheritdoc />
    public event Func<NotifyUserGeolocation, Task> NotifyDriverGeolocation = null!;

    /// <inheritdoc />
    public event Func<RideCreated, Task> RideCreated = null!;

    public async ValueTask StartConnectionAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        var userId = authState.User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value ?? Constants.Constants.Guest;

        _connection = new HubConnectionBuilder()
            .WithUrl($"{_configuration["FastRide:BaseUrl"]!}/api/?userId={userId}", options =>
            {
                if (_configuration["FastRide:BaseUrl"]!.Contains("ngrok-free.app"))
                {
                    options.Headers.Add("ngrok-skip-browser-warning", "true");
                }
            })
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

        _connection.On<RatingRequest>(SignalRConstants.ServerSendRatingRequest,
            async (payload) =>
            {
                if (SendRating != null!)
                {
                    await SendRating(payload);
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

        _connection.On<DriverAcceptRide>(SignalRConstants.ServerDriverAcceptRide,
            async (payload) =>
            {
                if (DriverAcceptRide != null!)
                {
                    await DriverAcceptRide(payload);
                }
            });

        _connection.On(SignalRConstants.ServerDriverRideAccepted,
            async () =>
            {
                if (DriverRideAccepted != null!)
                {
                    await DriverRideAccepted();
                }
            });

        _connection.On(SignalRConstants.ServerCancelRide,
            async () =>
            {
                if (CancelRide != null!)
                {
                    await CancelRide();
                }
            });

        _connection.On<Ride>(SignalRConstants.ServerNotifyState,
            async (x) =>
            {
                if (NotifyState != null!)
                {
                    await NotifyState(x);
                }
            });

        _connection.On(SignalRConstants.ServerNotifyDriverTimeout,
            async () =>
            {
                if (NotifyDriverTimeout != null!)
                {
                    await NotifyDriverTimeout();
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

    public async Task NotifyDriverArrivedAsync(string groupName)
    {
        await _connection.SendAsync(SignalRConstants.ClientDriverArrived, groupName);
    }

    public async Task AcceptRideAsync(string instanceId, string driverId, bool accepted)
    {
        await _connection.SendAsync(SignalRConstants.ClientDriverAcceptRide, instanceId, driverId, accepted);
    }

    public async Task CreateNewRideAsync(NewRideInput rideInput)
    {
        await _connection.SendAsync(SignalRConstants.ClientCreateNewRide, rideInput);
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.ClientLeaveUserFromGroup, userId, groupName);
    }

    public async Task ConfirmPriceCalculatedAsync(string instanceId, string priceConfirmed)
    {
        await _connection.SendAsync(SignalRConstants.ClientSendPriceCalculation, instanceId, priceConfirmed);
    }

    public async Task ConfirmPaymentAsync(string instanceId, bool paymentSuccess)
    {
        await _connection.SendAsync(SignalRConstants.ClientSendPaymentIntent, instanceId, paymentSuccess);
    }

    public async Task SendRatingAsync(string instanceId, int rating)
    {
        await _connection.SendAsync(SignalRConstants.ClientSendRatingRequest, instanceId, rating);
    }

    public async Task JoinUserInGroupAsync(string userId, string groupName)
    {
        await _connection.SendAsync(SignalRConstants.ClientJoinUserToGroup, userId, groupName);
    }

    public async Task CancelRideAsync(string groupName)
    {
        await _connection.SendAsync(SignalRConstants.ClientCancelRide, groupName);
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