using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.BackgroundService;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private SendCurrentGeolocationService SendCurrentGeolocationService { get; set; }

    [Inject] private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    public void Dispose()
    {
        OverlayState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        OverlayState.OnChange += StateHasChanged;

        var authState = await AuthenticationStateTask;

        if (authState.User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == UserType.Driver.ToString())
        {
            SendCurrentGeolocationService.StartExecuting();
        }
    }
}