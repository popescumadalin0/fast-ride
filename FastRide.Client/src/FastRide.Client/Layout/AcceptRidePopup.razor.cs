using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Layout;

public partial class AcceptRidePopup : IDisposable
{
    private bool _openAvailableRide;

    private bool _opened;

    private DriverRideInformation _ride;
    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        if (authState.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value ==
            UserType.Driver.ToString())
        {
            SignalRService.DriverAcceptRide += DriverAcceptRide;
        }


        await base.OnInitializedAsync();
    }

    private void OpenRide()
    {
        _openAvailableRide = !_openAvailableRide;

        StateHasChanged();
    }

    private async Task DriverAcceptRide(DriverAcceptRide arg)
    {
        var locationText = await LocationService.GetAddressByLatLongAsync(arg.DestinationGeolocation.Latitude,
            arg.DestinationGeolocation.Longitude);
        
        var distance = await .GetAddressByLatLongAsync(arg.DestinationGeolocation.Latitude,
            arg.DestinationGeolocation.Longitude);
        _ride = new DriverRideInformation()
        {
            Distance = distance,
            Destination = locationText,
            DestinationLocation = arg.DestinationGeolocation
        };
        OpenRide();
    }

    private async Task AcceptRideAsync()
    {
        _opened = false;
        
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        await SignalRService.AcceptRideAsync(new RideInformation()
        {
            Destination =  _ride.Destination,
            DestinationLocation =  _ride.DestinationLocation,
            DriverEmail = authState.User.Claims.SingleOrDefault(x => x.Type == "email")?.Value,
        });
    }
}