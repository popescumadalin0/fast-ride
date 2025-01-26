using System;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;

namespace FastRide.Client.Contracts;

public interface ISignalRService : IAsyncDisposable
{
    event Func<NotifyUserGeolocation, Task> NotifyDriverGeolocation;

    event Func<RideCreated, Task> RideCreated;

    event Func<PriceCalculated, Task> SendPriceCalculated;

    event Func<SendPaymentIntent, Task> SendPaymentIntentReceived;

    ValueTask StartConnectionAsync();

    ValueTask InitiateSignalRSubscribersAsync();

    Task RemoveUserFromGroupAsync(string userId, string groupName);

    Task ConfirmPriceCalculatedAsync(string instanceId, decimal priceConfirmed);

    Task ConfirmPaymentAsync(string instanceId, bool paymentSuccess);
    
    Task JoinUserInGroupAsync(string userId, string groupName);

    Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation);

    Task AcceptRideAsync(RideInformation rideInformation);

    Task CreateNewRideAsync(string groupName, NewRideInput rideInput);
}