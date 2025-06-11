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
    
    event Func<RatingRequest, Task> SendRating;

    event Func<UserIdentifier, Task> DriverRideAccepted;
    
    event Func<Task> CancelRide;
    
    event Func<Ride,Task> NotifyState;
    
    event Func<Task> NotifyDriverTimeout;

    ValueTask StartConnectionAsync();

    ValueTask InitiateSignalRSubscribersAsync();

    Task RemoveUserFromGroupAsync(string userId, string groupName);

    Task ConfirmPriceCalculatedAsync(string instanceId, string priceConfirmed);

    Task ConfirmPaymentAsync(string instanceId, bool paymentSuccess);
    
    Task SendRatingAsync(string instanceId, int rating);
    
    Task JoinUserInGroupAsync(string userId, string groupName);

    Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation);
    
    Task NotifyDriverArrivedAsync(string groupName, Geolocation geolocation);

    Task AcceptRideAsync(string instanceId, string userId, bool accepted);

    Task CreateNewRideAsync(NewRideInput rideInput);

    Task CancelRideAsync(string groupName);
}