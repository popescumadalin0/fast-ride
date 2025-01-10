using System;
using System.Threading.Tasks;
using FastRide.Client.Components;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace FastRide.Client.Layout;

public partial class NavMenu : IDisposable
{
    private bool _openProfileSettings;
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IDialogService DialogService { get; set; }
    
    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    protected override void OnInitialized()
    {
        DestinationState.OnChange += StateHasChanged;
        base.OnInitialized();
    }

    private async Task RideAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<PaymentConfirmationDialog>("Confirm payment", options);
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