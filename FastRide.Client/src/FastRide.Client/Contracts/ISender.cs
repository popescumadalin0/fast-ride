using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Contracts;

public interface ISender : IAsyncDisposable
{
    Task JoinUserInGroupAsync(string userId, string groupName);

    Task NotifyUserGeolocationAsync(string userId, string groupName, Geolocation geolocation);
}