using System;
using System.Threading.Tasks;
using FastRide.Client.Components;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.Service;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace FastRide.Client.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IDialogService DialogService { get; set; }
    
    [Inject] private ISignalRObserver SignalRObserver { get; set; }

    private bool _openProfileSettings;
    private bool OpenAvailableRide { get; set; }
    
    private RideInformation _availableRide;

    protected override void OnInitialized()
    {
        DestinationState.OnChange += StateHasChanged;
        SignalRObserver.AvailableRide += OpenRideAsync;
        base.OnInitialized();
    }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRObserver.AvailableRide -= OpenRideAsync;
    }

    private async Task RideAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<PaymentConfirmationDialog>("Confirm payment", options);
    }

    private Task OpenRideAsync(Server.Contracts.Ride ride)
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