using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Components;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using Geolocation = FastRide.Server.Contracts.Models.Geolocation;

namespace FastRide.Client.Layout;

public partial class StartRideButton : IAsyncDisposable, IBrowserViewportObserver
{
    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private CurrentPositionState CurrentPositionState { get; set; }

    [Inject] private IUserGroupService UserGroupService { get; set; }

    [Inject] private IBrowserViewportService BrowserViewportService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private ICurrentRideState CurrentRideState { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    public Guid Id { get; } = Guid.NewGuid();

    private int _width;

    private int _height;

    private bool _visible;

    public void OnOverlayClosed()
    {
        if (!_visible)
        {
        }
    }

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    public async ValueTask DisposeAsync()
    {
        DestinationState.OnChange -= StateHasChanged;

        SignalRService.RideCreated -= RideCreated;

        CurrentRideState.OnChange -= StateHasChanged;

        await BrowserViewportService.UnsubscribeAsync(this);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DestinationState.OnChange += StateHasChanged;

        SignalRService.RideCreated += RideCreated;
        
        CurrentRideState.OnChange += StateHasChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task RideAsync()
    {
        OverlayState.DataLoading = true;

        var authState = await AuthenticationStateTask;
        var groupName = await UserGroupService.GetCurrentUserGroupNameAsync();
        var email = authState.User.Claims.First(c => c.Type == "email").Value;
        var userId = authState.User.Claims.First(c => c.Type == "sub").Value;

        var currentLocation = CurrentPositionState.Geolocation;

        await SignalRService.CreateNewRideAsync(new NewRideInput()
        {
            Destination = new Geolocation()
            {
                Longitude = DestinationState.Geolocation.Longitude,
                Latitude = DestinationState.Geolocation.Latitude,
            },
            StartPoint = new Geolocation()
            {
                Latitude = currentLocation.Latitude,
                Longitude = currentLocation.Longitude
            },
            User = new UserIdentifier()
            {
                Email = email,
                NameIdentifier = userId
            },
            GroupName = groupName
        });
    }

    private async Task RideCreated(RideCreated rideCreated)
    {
        OverlayState.DataLoading = false;

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            Position = DialogPosition.Center,
            NoHeader = true,
            BackdropClick = false
        };

        if (_width <= 641)
        {
            options.CloseButton = true;
            options.FullScreen = true;
        }

        await DialogService.ShowAsync<PaymentConfirmationDialog>(string.Empty, options);
    }

    public Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        _width = browserViewportEventArgs.BrowserWindowSize.Width;
        _height = browserViewportEventArgs.BrowserWindowSize.Height;

        return InvokeAsync(StateHasChanged);
    }
}