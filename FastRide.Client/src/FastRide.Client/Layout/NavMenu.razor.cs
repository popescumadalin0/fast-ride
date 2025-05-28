using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Components;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;
using Ride = FastRide.Server.Contracts.Models.Ride;

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

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    public async ValueTask DisposeAsync()
    {
        DestinationState.OnChange -= DestinationStateOnOnChange;
        CurrentRideState.OnChange -= CurrentRideStateOnChange;
        SignalRService.SendRating -= OpenRatingDialog;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            SignalRService.DriverRideAccepted -= DriverAcceptedRide;
            SignalRService.NotifyState -= UpdateRideState;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            SignalRService.DriverRideAccepted += DriverAcceptedRide;
            SignalRService.NotifyState += UpdateRideState;
        }

        DestinationState.OnChange += DestinationStateOnOnChange;
        SignalRService.CancelRide += CancelRideAsync;
        CurrentRideState.OnChange += CurrentRideStateOnChange;
        SignalRService.SendRating += OpenRatingDialog;

        await DialogService.ShowAsync<RatingDialog>("TEst your driver", new DialogOptions
        {
            CloseOnEscapeKey = true,
            Position = DialogPosition.Center,
        });


        await base.OnInitializedAsync();
    }

    private Task CurrentRideStateOnChange()
    {
        StateHasChanged();

        return Task.CompletedTask;
    }

    private async Task UpdateRideState(Ride ride)
    {
        if (ride.Status is InternRideStatus.None &&
            CurrentRideState.InstanceId != null)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            await SignalRService.RemoveUserFromGroupAsync(
                authState.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value, CurrentRideState.InstanceId);

            var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();
            await SignalRService.JoinUserInGroupAsync(
                authState.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value, groupName);
        }

        await CurrentRideState.UpdateState(ride);
    }

    private async Task DestinationStateOnOnChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task CancelRideAsync()
    {
        Snackbar.Add("Ride Cancelled", Severity.Error);
        await SignalRService.CancelRideAsync(await UserGroupService.GetCurrentUserGroupNameAsync());
    }

    private async Task DriverAcceptedRide(UserIdentifier userId)
    {
        var dialogOptions = new DialogOptions
        {
            CloseOnEscapeKey = true,
            Position = DialogPosition.Center,
        };
        var dialogParameters = new DialogParameters<DriverInformationDialog>
        {
            { x => x.UserIdentifier, userId }
        };

        await DialogService.ShowAsync<DriverInformationDialog>("Your driver is on the way!", dialogParameters,
            dialogOptions);
    }

    private async Task OpenRatingDialog(RatingRequest request)
    {
        var dialogParameters = new DialogParameters<RatingDialog>
        {
            { x => x.InstanceId, request.InstanceId }
        };

        var dialogOptions = new DialogOptions
        {
            CloseOnEscapeKey = true,
            Position = DialogPosition.Center,
        };

        await DialogService.ShowAsync<RatingDialog>("Rate your driver", dialogParameters, dialogOptions);
    }

    private async Task LogOutAsync()
    {
        OverlayState.DataLoading = true;
        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        DestinationState.Geolocation = null;
        var userId = auth.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();
        await SignalRService.RemoveUserFromGroupAsync(userId, groupName);
        await SignalRService.JoinUserInGroupAsync(Constants.Constants.Guest, groupName);
        Navigation.NavigateToLogout("authentication/logout");
        OverlayState.DataLoading = false;
    }

    private async Task LogInAsync()
    {
        OverlayState.DataLoading = true;
        DestinationState.Geolocation = null;
        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();
        await SignalRService.RemoveUserFromGroupAsync(Constants.Constants.Guest, groupName);
        Navigation.NavigateToLogout("authentication/login");
        OverlayState.DataLoading = false;
    }
}