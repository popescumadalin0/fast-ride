using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts;

namespace FastRide.Client.Service;

public interface ISignalRObserver : IAsyncDisposable
{
    /// <summary>
    /// 
    /// </summary>
    event Func<Ride, Task> AvailableRiders;

    /// <summary>
    /// 
    /// </summary>
    event Func<Ride, Task> AvailableRides;
}