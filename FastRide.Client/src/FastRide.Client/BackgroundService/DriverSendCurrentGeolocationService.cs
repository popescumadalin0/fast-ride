using System;
using System.Linq;
using System.Timers;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Timer = System.Timers.Timer;

namespace FastRide.Client.BackgroundService;

public class JobExecutedEventArgs : EventArgs;

public class DriverSendCurrentGeolocationService : IDisposable
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IGeolocationService _geolocationService;
    private readonly ISender _sender;
    private readonly IUserGroupService _userGroupService;
    private bool _running;

    private Timer _timer;

    public DriverSendCurrentGeolocationService(ISender sender,
        IGeolocationService geolocationService, IUserGroupService userGroupService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _geolocationService = geolocationService;
        _userGroupService = userGroupService;
        _authenticationStateProvider = authenticationStateProvider;
        _sender = sender;
    }

    public void Dispose()
    {
        if (_running)
        {
            _timer.Dispose();
        }
    }

    public event EventHandler<JobExecutedEventArgs> JobExecuted;

    private void OnJobExecuted()
    {
        JobExecuted?.Invoke(this, new JobExecutedEventArgs());
    }

    public void StartExecuting()
    {
        if (_running) return;
        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.Elapsed += HandleTimer;
        _timer.AutoReset = true;
        _timer.Enabled = true;

        _running = true;
    }

    private void HandleTimer(object source, ElapsedEventArgs e)
    {
        var auth = _authenticationStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
        var userId = auth.User.Claims.Single(x => x.Type == "sub").Value;
        var groupName = _userGroupService.GetCurrentUserGroupNameAsync().GetAwaiter().GetResult();

        var geolocation = _geolocationService.GetCoordonatesAsync().GetAwaiter().GetResult();
        _sender.NotifyUserGeolocationAsync(userId,
                groupName,
                new Geolocation()
                {
                    Latitude = geolocation.Latitude,
                    Longitude = geolocation.Longitude,
                })
            .GetAwaiter()
            .GetResult();

        OnJobExecuted();
    }
}