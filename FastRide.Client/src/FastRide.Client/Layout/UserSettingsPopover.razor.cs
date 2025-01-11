using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FastRide.Client.Layout;

public partial class UserSettingsPopover
{
    private bool _openProfileSettings;

    [Parameter] public AuthenticationState Context { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    private async Task OpenChangeUserSettingsAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<ChangeUserSettingsDialog>("Change user settings", options);

        _openProfileSettings = false;
    }

    private void OpenAccountMenu()
    {
        _openProfileSettings = !_openProfileSettings;
    }
}