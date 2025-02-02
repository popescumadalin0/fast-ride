using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.BackgroundService;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client;

public partial class App : IAsyncDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private DriverSendCurrentGeolocationService DriverSendCurrentGeolocationService { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject] private IUserGroupService UserGroupService { get; set; }

    public async ValueTask DisposeAsync()
    {
        await SignalRService.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await SignalRService.StartConnectionAsync();
        await SignalRService.InitiateSignalRSubscribersAsync();

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();

        var userId = authState.User.Identity!.IsAuthenticated
            ? authState.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
            : Constants.Constants.Guest;

        await SignalRService.JoinUserInGroupAsync(userId, groupName);

        if (authState.User.Identity!.IsAuthenticated &&
            authState.User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == UserType.Driver.ToString())
        {
            DriverSendCurrentGeolocationService.StartExecuting();
        }
    }
}