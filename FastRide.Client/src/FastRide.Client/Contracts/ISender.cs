using System;
using System.Threading.Tasks;
using FastRide.Client.Models;
using Geolocation = FastRide.Server.Contracts.Models.Geolocation;

namespace FastRide.Client.Contracts;

public interface ISender : IAsyncDisposable
{
    Task JoinUserInGroupAsync(string userId, string groupName);

    Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation);
    
    Task AcceptRideAsync(RideInformation rideInformation);
}