using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace FastRide.Client.Layout;

public partial class NavMenu : IAsyncDisposable
{
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private IUserGroupService UserGroupService { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject] private ICurrentRideState CurrentRideState { get; set; }
    
    [Inject] private ISnackbar  Snackbar { get; set; }

    public async ValueTask DisposeAsync()
    {
        DestinationState.OnChange -= StateHasChanged;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            SignalRService.DriverRideAccepted -= DriverAcceptedRide;
            SignalRService.NotifyState -= CurrentRideState.UpdateState;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            SignalRService.DriverRideAccepted += DriverAcceptedRide;
            SignalRService.NotifyState += CurrentRideState.UpdateState;
        }

        DestinationState.OnChange += StateHasChanged;
        SignalRService.CancelRide += CancelRideAsync;

        await base.OnInitializedAsync();
    }

    private async Task CancelRideAsync()
    {
        Snackbar.Add("Ride Cancelled", Severity.Error);
        await SignalRService.CancelRideAsync(await UserGroupService.GetCurrentUserGroupNameAsync());
    }

    private async Task DriverAcceptedRide()
    {
   
    }

    private async Task LogOutAsync()
    {
        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        DestinationState.Geolocation = null;
        var userId = auth.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();
        await SignalRService.RemoveUserFromGroupAsync(userId, groupName);
        await SignalRService.JoinUserInGroupAsync(Constants.Constants.Guest, groupName);
        Navigation.NavigateToLogout("authentication/logout");
    }

    private async Task LogInAsync()
    {
        DestinationState.Geolocation = null;
        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();
        await SignalRService.RemoveUserFromGroupAsync(Constants.Constants.Guest, groupName);
        Navigation.NavigateToLogout("authentication/login");
    }
}