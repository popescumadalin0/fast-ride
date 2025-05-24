using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase, IAsyncDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private ICurrentRideState CurrentRideState { get; set; }

    [Inject] private CurrentPositionState CurrentPositionState { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    [Inject] private IConfiguration Configuration { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }

    private OpenStreetMapResponse _selectedAddress = new();

    private Map _map = new();

    private bool _isInitialized;

    public async ValueTask DisposeAsync()
    {
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
        CurrentPositionState.OnChange -= CurrentPositionStateOnChangeAsync;
        CurrentRideState.OnChange -= CurrentRideStateOnOnChange;
        DestinationState.OnChange -= DestinationStateOnChangeAsync;
    }

    protected override async Task OnInitializedAsync()
    {
        SignalRService.NotifyDriverGeolocation += NotifyDriverGeolocationAsync;

        CurrentPositionState.OnChange += CurrentPositionStateOnChangeAsync;

        CurrentRideState.OnChange += CurrentRideStateOnOnChange;

        DestinationState.OnChange += DestinationStateOnChangeAsync;

        StateHasChanged();
    }

    private async Task CurrentRideStateOnOnChange()
    {
        if (CurrentRideState.State == RideStatus.None)
        {
            await _map.RemoveRouteAsync();
            DestinationState.Geolocation = null;
        }

        StateHasChanged();
    }

    private async Task<IEnumerable<OpenStreetMapResponse>> Search(string value, CancellationToken token)
    {
        var suggestions = await LocationService.GetAddressesBySuggestions(_map.Locality, value);

        return suggestions;
    }

    private async Task NotifyDriverGeolocationAsync(NotifyUserGeolocation input)
    {
        if (!_isInitialized)
        {
            return;
        }

        if (CurrentRideState.State < RideStatus.DriverGoingToDestination)
        {
            var authState = await AuthenticationState;
            if (input.UserId != authState.User?.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value)
            {
                await _map.SetDriverLocationAsync(input.UserId, input.Geolocation);
            }
        }
    }

    private async Task CurrentPositionStateOnChangeAsync()
    {
        if (!_isInitialized)
        {
            return;
        }

        var authState = await AuthenticationState;
        var userId = authState.User.Identity?.IsAuthenticated ?? false
            ? authState.User.Claims.Single(x => x.Type == "sub").Value
            : Guid.Empty.ToString();

        var role = authState.User.Identity?.IsAuthenticated ?? false
            ? authState.User.Claims.Single(x => x.Type == ClaimTypes.Role).Value
            : UserType.User.ToString();

        bool inCar;

        if (role == UserType.User.ToString())
        {
            inCar = CurrentRideState.State is RideStatus.GoingToUser or RideStatus.GoingToDestination;
        }
        else
        {
            inCar = CurrentRideState.State is RideStatus.DriverGoingToDestination or RideStatus.DriverGoingToUser;
        }

        await _map.SetUserLocationAsync(userId, CurrentPositionState.Geolocation,
            inCar);

        if (inCar)
        {
            await _map.DrawRouteAsync(CurrentPositionState.Geolocation, DestinationState.Geolocation);
        }

        StateHasChanged();
    }

    private async Task OnMapRenderedAsync()
    {
        await _map.SetPinLocationAsync(DestinationState.Geolocation);
        _isInitialized = true;
    }

    private void MapClicked(Geolocation obj)
    {
        if (CurrentRideState.State == RideStatus.None)
        {
            DestinationState.Geolocation = obj;
        }
    }

    private async Task DestinationStateOnChangeAsync()
    {
        if (!_isInitialized)
        {
            return;
        }

        await _map.SetPinLocationAsync(DestinationState.Geolocation);
    }

    private void OnAutoCompleteValueChanged(OpenStreetMapResponse obj)
    {
        _selectedAddress = obj;
        DestinationState.Geolocation = new Geolocation()
        {
            Latitude = obj.lat,
            Longitude = obj.lon
        };
    }
}