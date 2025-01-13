using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.BackgroundService;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private DriverSendCurrentGeolocationService DriverSendCurrentGeolocationService { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }

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
            DriverSendCurrentGeolocationService.StartExecuting();
        }
    }
}