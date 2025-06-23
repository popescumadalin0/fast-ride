using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.BackgroundService;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client;

public partial class App : IAsyncDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private CalculateCurrentGeolocationService CalculateCurrentGeolocationService { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject] private IUserGroupService UserGroupService { get; set; }
    
    [Inject] private OverlayState OverlayState { get; set; }

    public async ValueTask DisposeAsync()
    {
        await SignalRService.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        OverlayState.DataLoading = true;
        await SignalRService.StartConnectionAsync();
        await SignalRService.InitiateSignalRSubscribersAsync();

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();

        var userId = authState.User.Identity?.IsAuthenticated ?? false
            ? authState.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
            : Constants.Constants.Guest;

        await SignalRService.JoinUserInGroupAsync(userId, groupName);

        CalculateCurrentGeolocationService.StartExecutingAsync();
        OverlayState.DataLoading = false;
    }
}