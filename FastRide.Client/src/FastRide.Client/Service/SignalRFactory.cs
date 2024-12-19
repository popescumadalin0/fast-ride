using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.Senders;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace FastRide.Client.Service;

public class SignalRFactory : ISignalRFactory
{
    private readonly ILogger<SignalRFactory> _logger;

    private readonly Task<AuthenticationState> _authenticationStateTask;

    private readonly IEnumerable<ISender> _senders;

    private readonly IEnumerable<IObserver> _observers;

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
        var groupName = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var sender = _senders.First(x => x.UserType == Enum.Parse<UserType>(groupName!));

        return sender;
    }

    public async Task<IObserver> GetObserverAsync()
    {
        var authState = await _authenticationStateTask;
        var groupName = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var observer = _observers.First(x => x.UserType == Enum.Parse<UserType>(groupName!));

        return observer;
    }
}