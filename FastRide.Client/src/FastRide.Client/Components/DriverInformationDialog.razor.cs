using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class DriverInformationDialog
{
    [Parameter] public UserIdentifier UserIdentifier { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; }

    private User _user;

    protected override async Task OnInitializedAsync()
    {
        var user = await FastRideApiClient.GetUserAsync(UserIdentifier);

        if (!user.Success)
        {
            SnackBar.Add("Cannot see the driver!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
            return;
        }

        _user = user.Response;

        await base.OnInitializedAsync();

        StateHasChanged();
    }
}