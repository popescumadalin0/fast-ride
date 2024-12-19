using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Contracts.Enums;

namespace FastRide.Client.Contracts;

public interface ISender : IAsyncDisposable
{
    UserType UserType { get; }

    Task JoinUserInGroupAsync(string userId);
}