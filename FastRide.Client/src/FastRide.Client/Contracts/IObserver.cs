using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Contracts;

public interface IObserver : IAsyncDisposable
{                            
    UserType UserType { get; }
}