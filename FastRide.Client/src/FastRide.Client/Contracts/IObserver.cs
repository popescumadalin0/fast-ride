using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Contracts;

public interface IObserver : IAsyncDisposable
{
    event Func<string, Geolocation, Task> NotifyUserGeolocation;
}