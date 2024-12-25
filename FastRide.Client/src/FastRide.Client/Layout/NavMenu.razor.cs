using System;
using System.Threading.Tasks;
using FastRide.Client.Components;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;
using Ride = FastRide.Server.Contracts.Models.Ride;

namespace FastRide.Client.Layout;

public partial class NavMenu : IDisposable
{
    private RideInformation _availableRide;

    private bool _openProfileSettings;
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private IObserver Observer { get; set; }
    private bool OpenAvailableRide { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        //Observer.AvailableRide -= OpenRideAsync;
    }

    protected override void OnInitialized()
    {
        DestinationState.OnChange += StateHasChanged;
        //Observer.AvailableRide += OpenRideAsync;
        base.OnInitialized();
    }

    private async Task RideAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<PaymentConfirmationDialog>("Confirm payment", options);
    }

    private Task OpenRideAsync(Ride ride)
    {
        OpenAvailableRide = !OpenAvailableRide;
        _availableRide =
            new RideInformation()
            {
                Destination = "tsetasdfasdf asdfasdfasd fasdfasdfasdf asdfasdfas dfasdf"
            };
        StateHasChanged();

        return Task.CompletedTask;
    }

    private void OpenAccountMenu()
    {
        _openProfileSettings = !_openProfileSettings;
    }

    private async Task OpenChangeUserSettingsAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<ChangeUserSettingsDialog>("Change user settings", options);

        _openProfileSettings = false;
    }

    private void LogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
}