using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class ChangeUserSettingsDialog : ComponentBase
{
    private string _phoneNumber;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }
    
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }

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

        var auth = await AuthenticationStateTask;
        
        var response = await FastRideApiClient.UpdateUserAsync(new UpdateUserPayload()
        {
            User = new UserIdentifier()
            {
                NameIdentifier = auth.User.Claims.Single(x => x.Type == "sub").Value,
                Email = auth.User.Claims.Single(x => x.Type == "email").Value
            },
            PhoneNumber = _phoneNumber,
            PictureUrl = auth.User.Claims.Single(x => x.Type == "picture").Value
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