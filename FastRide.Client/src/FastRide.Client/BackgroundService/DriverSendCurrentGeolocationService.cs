using System;
using System.Linq;
using System.Threading.Tasks;
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
    private readonly ISignalRService _signalRService;
    private readonly IUserGroupService _userGroupService;
    private bool _running;

    private Timer _timer;

    public DriverSendCurrentGeolocationService(ISignalRService signalRService,
        IGeolocationService geolocationService, IUserGroupService userGroupService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _geolocationService = geolocationService;
        _userGroupService = userGroupService;
        _authenticationStateProvider = authenticationStateProvider;
        _signalRService = signalRService;
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
        _timer.Interval = 5000;
        _timer.Elapsed += HandleTimer;
        _timer.AutoReset = true;
        _timer.Enabled = true;

        _running = true;
    }

    private void HandleTimer(object source, ElapsedEventArgs e)
    {
        _geolocationService.CoordinatesChanged += OnCoordinatesChanged;
        _geolocationService.RequestGeoLocationAsync().GetAwaiter().GetResult();
    }

    private async ValueTask OnCoordinatesChanged(Geolocation geolocation)
    {
        var auth = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var userId = auth.User.Claims.Single(x => x.Type == "sub").Value;
        var groupName = await _userGroupService.GetCurrentUserGroupNameAsync();

        await _signalRService.NotifyUserGeolocationAsync(userId,
            groupName,
            geolocation);
        
        _geolocationService.CoordinatesChanged -= OnCoordinatesChanged;
        OnJobExecuted();
    }
}