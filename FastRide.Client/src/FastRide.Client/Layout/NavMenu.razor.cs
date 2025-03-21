using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }
    
    [Inject] private IUserGroupService UserGroupService { get; set; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    protected override void OnInitialized()
    {
        DestinationState.OnChange += StateHasChanged;
        base.OnInitialized();
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