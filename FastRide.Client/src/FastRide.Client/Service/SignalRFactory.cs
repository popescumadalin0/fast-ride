using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace FastRide.Client.Service;

public class SignalRFactory : ISignalRFactory
{
    private readonly Task<AuthenticationState> _authenticationStateTask;
    private readonly ILogger<SignalRFactory> _logger;

    private readonly IEnumerable<IObserver> _observers;

    private readonly IEnumerable<ISender> _senders;

    public SignalRFactory(IEnumerable<ISender> senders,
        ILogger<SignalRFactory> logger,
        Task<AuthenticationState> authenticationStateTask,
        IEnumerable<IObserver> observers)
    {
        _senders = senders;
        _logger = logger;
        _authenticationStateTask = authenticationStateTask;
        _observers = observers;
    }

    public async Task<ISender> GetSenderAsync()
    {
        var authState = await _authenticationStateTask;
        var userType = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var sender = _senders.First(x => x.UserType == Enum.Parse<UserType>(userType!));

        return sender;
    }

    public async Task<IObserver> GetObserverAsync()
    {
        var authState = await _authenticationStateTask;
        var userType = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var observer = _observers.First(x => x.UserType == Enum.Parse<UserType>(userType!));

        return observer;
    }
}