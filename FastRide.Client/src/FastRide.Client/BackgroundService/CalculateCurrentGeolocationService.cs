using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components.Authorization;

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

    private PeriodicTimer _timer;
    private CancellationTokenSource _cts;

    private bool _running;

    private bool _arrivedSent;

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

        await SaveCurrentGeolocationAsync();

        _running = true;
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            await HandleTimerAsync();
        }
    }

    private async ValueTask HandleTimerAsync()
    {
        await SaveCurrentGeolocationAsync();
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

            if (_currentRideState.State != RideStatus.None && _currentRideState.State != RideStatus.Finished)
            {
                const double tolerance = 0.0005;
                if (Math.Abs(geolocation.Latitude - _destinationState.Geolocation.Latitude) < tolerance &&
                    Math.Abs(geolocation.Longitude - _destinationState.Geolocation.Longitude) < tolerance &&
                    !_arrivedSent)
                {
                    await _signalRService.NotifyDriverArrivedAsync(groupName);
                    _arrivedSent = true;
                }
                else
                {
                    _arrivedSent = false;
                }
            }
        }

        _currentPositionState.Geolocation = geolocation;
    }
}