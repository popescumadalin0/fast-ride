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
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    private bool _openProfileSettings;
    private bool OpenAvailableRides { get; set; }

    protected override void OnInitialized()
    {
        DestinationState.OnChange += StateHasChanged;
        base.OnInitialized();
    }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    private async Task DriveAsync()
    {
    }

    private void OpenRides()
    {
        OpenAvailableRides = !OpenAvailableRides;
        StateHasChanged();
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