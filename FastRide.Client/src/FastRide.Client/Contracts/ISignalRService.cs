using System;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;
using Geolocation = FastRide.Client.Models.Geolocation;

namespace FastRide.Client.Contracts;

public interface ISignalRService : IAsyncDisposable
{
    event Func<string, Geolocation, Task> NotifyDriverGeolocation;

    event Func<string, Task> RideCreated;

    ValueTask StartConnectionAsync();

    Task RemoveUserFromGroupAsync(string userId, string groupName);

    Task JoinUserInGroupAsync(string userId, string groupName);

    Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation);

    Task AcceptRideAsync(RideInformation rideInformation);

    Task CreateNewRideAsync(string groupName, NewRideInput rideInput);
}