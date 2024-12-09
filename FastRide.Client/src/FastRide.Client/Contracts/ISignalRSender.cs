using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts;

namespace FastRide.Client.Contracts;

public interface ISignalRSender : IAsyncDisposable
{
    Task AcceptRideAsync(Ride ride);

    Task BookRideAsync(Ride ride);
}