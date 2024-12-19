using System.Threading.Tasks;
using FastRide.Client.State;
using FastRide.Server.Contracts;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class ChangeUserSettingsDialog : ComponentBase
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    private string _phoneNumber;

    protected override async Task OnInitializedAsync()
    {
        OverlayState.DataLoading = true;
        var userInformation = await FastRideApiClient.GetCurrentUserAsync();
        
        OverlayState.DataLoading = false;
        
        if (!userInformation.Success)
        {
            Snackbar.Add(userInformation.ResponseMessage, Severity.Error);
            return;
        }

        _phoneNumber = userInformation.Response.PhoneNumber;
        
        StateHasChanged();
    }

    private async Task Submit()
    {
        OverlayState.DataLoading = true;

        var response = await FastRideApiClient.UpdateUserAsync(new UpdateUserPayload()
        {
            PhoneNumber = _phoneNumber
        });

        OverlayState.DataLoading = false;

        if (!response.Success)
        {
            Snackbar.Add(response.ResponseMessage, Severity.Error);
            return;
        }

        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog.Cancel();
}