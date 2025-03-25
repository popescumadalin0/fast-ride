using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Timer = System.Timers.Timer;

namespace FastRide.Client.BackgroundService;

public class JobExecutedEventArgs : EventArgs;

public class CalculateCurrentGeolocationService : IDisposable
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IGeolocationService _geolocationService;
    private readonly ISignalRService _signalRService;
    private readonly IUserGroupService _userGroupService;
    private readonly CurrentPositionState _currentPositionState;
    private bool _running;

    private Timer _timer;

    public CalculateCurrentGeolocationService(ISignalRService signalRService,
        IGeolocationService geolocationService, IUserGroupService userGroupService,
        AuthenticationStateProvider authenticationStateProvider, CurrentPositionState currentPositionState)
    {
        _geolocationService = geolocationService;
        _userGroupService = userGroupService;
        _authenticationStateProvider = authenticationStateProvider;
        _currentPositionState = currentPositionState;
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

    private void HandleTimer(object source = null, ElapsedEventArgs e = null)
    {
        SaveCurrentGeolocationAsync().GetAwaiter().GetResult();
    }

    private async Task SaveCurrentGeolocationAsync()
    {
        var auth = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var geolocation = await _geolocationService.GetGeolocationAsync();

        if ((auth.User.Identity?.IsAuthenticated ?? false) &&
            auth.User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == UserType.Driver.ToString())
        {
            var userId = auth.User.Claims.Single(x => x.Type == "sub").Value;
            var groupName = await _userGroupService.GetCurrentUserGroupNameAsync();

            await _signalRService.NotifyUserGeolocationAsync(userId,
                groupName,
                geolocation);
        }

        _currentPositionState.Geolocation = geolocation;

        OnJobExecuted();
    }
}