using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Timer = System.Timers.Timer;

namespace FastRide.Client.BackgroundService;

public class CalculateCurrentGeolocationService : IDisposable
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IGeolocationService _geolocationService;
    private readonly ISignalRService _signalRService;
    private readonly IUserGroupService _userGroupService;
    private readonly CurrentPositionState _currentPositionState;
    private readonly ICurrentRideState _currentRideState;
    private readonly DestinationState _destinationState;
    private bool _running;

    private PeriodicTimer _timer;
    private CancellationTokenSource _cts;

    public CalculateCurrentGeolocationService(ISignalRService signalRService,
        IGeolocationService geolocationService, IUserGroupService userGroupService,
        AuthenticationStateProvider authenticationStateProvider, CurrentPositionState currentPositionState,
        ICurrentRideState currentRideState, DestinationState destinationState)
    {
        _geolocationService = geolocationService;
        _userGroupService = userGroupService;
        _authenticationStateProvider = authenticationStateProvider;
        _currentPositionState = currentPositionState;
        _currentRideState = currentRideState;
        _destinationState = destinationState;
        _signalRService = signalRService;
    }

    public void Dispose()
    {
        if (!_running) return;
        _cts.Dispose();
        _timer.Dispose();
    }

    public async Task StartExecutingAsync()
    {
        if (_running) return;

        _running = true;
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(2.5));

        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            await HandleTimerAsync();
        }
    }

    public async Task StopExecutingAsync()
    {
        _running = false;
        await _cts.CancelAsync();
    }

    private async ValueTask HandleTimerAsync()
    {
        await SaveCurrentGeolocationAsync();

        if (_currentRideState.State is RideStatus.Finished or RideStatus.Cancelled)
        {
            _currentRideState.ResetState();
        }
    }

    private async Task SaveCurrentGeolocationAsync()
    {
        var auth = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var geolocation = await _geolocationService.GetGeolocationAsync();
        var groupName = await _userGroupService.GetCurrentUserGroupNameAsync();

        if ((auth.User.Identity?.IsAuthenticated ?? false) &&
            auth.User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == UserType.Driver.ToString())
        {
            var userId = auth.User.Claims.Single(x => x.Type == "sub").Value;

            await _signalRService.NotifyUserGeolocationAsync(userId,
                groupName,
                geolocation);
        }

        _currentPositionState.Geolocation = geolocation;

        if (_currentRideState.State is not RideStatus.None)
        {
            const double tolerance = 0.00015;
            if (Math.Abs(geolocation.Latitude - _destinationState.Geolocation.Latitude) < tolerance &&
                Math.Abs(geolocation.Longitude - _destinationState.Geolocation.Longitude) < tolerance)
            {
                await _signalRService.NotifyDriverArrivedAsync(groupName);
            }
        }
    }
}