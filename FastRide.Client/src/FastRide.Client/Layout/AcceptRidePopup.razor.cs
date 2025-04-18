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

    private DriverRideInformation _ride;
    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    [Inject] private IUserGroupService UserGroupService { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject] private ICurrentRideState CurrentRideState { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        CurrentRideState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverTimeout += CurrentRideStateOnOnChange;
        SignalRService.DriverAcceptRide -= DriverNewRide;
    }

    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;
        
        CurrentRideState.OnChange += StateHasChanged;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        if (authState.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value ==
            UserType.Driver.ToString())
        {
            SignalRService.DriverAcceptRide += DriverNewRide;

            SignalRService.NotifyDriverTimeout += CurrentRideStateOnOnChange;
        }


        await base.OnInitializedAsync();
    }

    private Task CurrentRideStateOnOnChange()
    {
        OpenRide();
        _ride = null;
        StateHasChanged();
        
        return Task.CompletedTask;
    }

    private void OpenRide()
    {
        _openAvailableRide = !_openAvailableRide;

        StateHasChanged();
    }

    private async Task DriverNewRide(DriverAcceptRide arg)
    {
        var locationText = await LocationService.GetAddressByLatLongAsync(arg.DestinationGeolocation.Latitude,
            arg.DestinationGeolocation.Longitude);

        _ride = new DriverRideInformation()
        {
            Distance = arg.Distance,
            Destination = locationText,
            DestinationLocation = arg.DestinationGeolocation,
            InstanceId = arg.InstanceId,
        };
        OpenRide();
    }

    private async Task AcceptRideAsync()
    {
        _openAvailableRide = false;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        await SignalRService.AcceptRideAsync(_ride.InstanceId,
            authState.User.Claims.SingleOrDefault(x => x.Type == "sub")?.Value, true);
        _ride = null;
        await SignalRService.RemoveUserFromGroupAsync(
            authState.User.Claims.SingleOrDefault(x => x.Type == "sub")?.Value,
            await UserGroupService.GetCurrentUserGroupNameAsync());
        await SignalRService.JoinUserInGroupAsync(authState.User.Claims.SingleOrDefault(x => x.Type == "sub")?.Value,
            _ride!.InstanceId);
    }

    private async Task DiscardRideAsync()
    {
        _openAvailableRide = false;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        await SignalRService.AcceptRideAsync(_ride.InstanceId,
            authState.User.Claims.SingleOrDefault(x => x.Type == "sub")?.Value, false);
        _ride = null;
    }
}