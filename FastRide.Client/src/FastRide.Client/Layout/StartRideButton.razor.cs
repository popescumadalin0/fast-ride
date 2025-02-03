using System;
using System.Linq;
using System.Security.Claims;
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

    [Inject] private IGeolocationService GeolocationService { get; set; }

    [Inject] private IBrowserViewportService BrowserViewportService { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    public Guid Id { get; } = Guid.NewGuid();

    private int _width;

    private int _height;

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    public async ValueTask DisposeAsync()
    {
        DestinationState.OnChange -= StateHasChanged;

        SignalRService.RideCreated -= RemoveUserFromGroups;

        await BrowserViewportService.UnsubscribeAsync(this);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DestinationState.OnChange += StateHasChanged;

        SignalRService.RideCreated += RemoveUserFromGroups;
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
        var authState = await AuthenticationStateTask;
        var groupName = authState.User.Claims.First(c => c.Type == ClaimTypes.GroupSid).Value;
        var email = authState.User.Claims.First(c => c.Type == "email").Value;
        var userId = authState.User.Claims.First(c => c.Type == "sub").Value;

        var currentLocation = await GeolocationService.GetGeolocationAsync();

        await SignalRService.CreateNewRideAsync(groupName, new NewRideInput()
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
            }
        });
    }

    private async Task RemoveUserFromGroups(RideCreated rideCreated)
    {
        //todo: this when the payment is made
        /*var groupName = AuthenticationState.User.Claims.First(c => c.Type == ClaimTypes.GroupSid).Value;
        var userId = AuthenticationState.User.Claims.First(c => c.Type == "sub").Value;
        await Sender.RemoveUserFromGroupAsync(userId, groupName);*/

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