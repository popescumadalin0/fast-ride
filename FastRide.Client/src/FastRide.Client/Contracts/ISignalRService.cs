using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;

namespace FastRide.Client.Contracts;

public interface ISignalRService : IAsyncDisposable
{
    event Func<NotifyUserGeolocation, Task> NotifyDriverGeolocation;

    event Func<RideCreated, Task> RideCreated;

    event Func<PriceCalculated, Task> SendPriceCalculated;

    event Func<SendPaymentIntent, Task> SendPaymentIntentReceived;

    event Func<DriverAcceptRide, Task> DriverAcceptRide;

    event Func<Task> DriverRideAccepted;
    
    event Func<Task> CancelRide;

    ValueTask StartConnectionAsync();

    ValueTask InitiateSignalRSubscribersAsync();

    Task RemoveUserFromGroupAsync(string userId, string groupName);

    Task ConfirmPriceCalculatedAsync(string instanceId, string priceConfirmed);

    Task ConfirmPaymentAsync(string instanceId, bool paymentSuccess);
    
    Task JoinUserInGroupAsync(string userId, string groupName);

    Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation);
    
    Task NotifyDriverArrivedAsync(string groupName);

    Task AcceptRideAsync(string instanceId, string driverId, bool accepted);

    Task CreateNewRideAsync(NewRideInput rideInput);

    Task CancelRideAsync(string groupName);
}