using System;
using System.Collections.Generic;
using System.Linq;
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

    private readonly Dictionary<string, Geolocation> _drivers = new();

    private Map _map = new();

    private string _userId;

    public async ValueTask DisposeAsync()
    {
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
        CurrentPositionState.OnChange -= CurrentPositionStateOnChangeAsync;
        CurrentRideState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        SignalRService.NotifyDriverGeolocation += NotifyDriverGeolocationAsync;

        CurrentPositionState.OnChange += CurrentPositionStateOnChangeAsync;

        CurrentRideState.OnChange += StateHasChanged;

        var authState = await AuthenticationState;
        _userId = authState.User.Identity?.IsAuthenticated ?? false
            ? authState.User.Claims.Single(x => x.Type == "sub").Value
            : Guid.Empty.ToString();

        StateHasChanged();
    }

    private async Task<IEnumerable<OpenStreetMapResponse>> Search(string value, CancellationToken token)
    {
        var suggestions = await LocationService.GetAddressesBySuggestions(_map.Locality, value);

        return suggestions;
    }

    private async Task NotifyDriverGeolocationAsync(NotifyUserGeolocation input)
    {
        var authState = await AuthenticationState;
        if (input.UserId != authState.User?.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value)
        {
            _drivers[input.UserId] = new Geolocation()
            {
                Longitude = input.Geolocation.Longitude,
                Latitude = input.Geolocation.Latitude,
            };
        }
    }

    private async Task CurrentPositionStateOnChangeAsync()
    {
        StateHasChanged();
    }


    private void MapClicked(Geolocation obj)
    {
        if (CurrentRideState.State == RideStatus.None)
        {
            DestinationState.Geolocation = obj;
        }
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