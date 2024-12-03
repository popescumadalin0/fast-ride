using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Client.State;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class AvailableRidesPopup : IDisposable
{
    [Parameter] public bool Opened { get; set; }

    [Parameter] public EventCallback<bool> OpenedChanged { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    private List<RideInformation> _availableRides;

    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;

        _availableRides = new List<RideInformation>()
        {
            new RideInformation()
            {
                Destination = "tsetasdfasdf asdfasdfasd fasdfasdfasdf asdfasdfas dfasdf"
            },
            new RideInformation()
            {
                Destination = "tset"
            },
            new RideInformation()
            {
                Destination = "tset"
            }
        };

        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    private async Task AcceptRideAsync(RideInformation ride)
    {
        Opened = false;
        await OpenedChanged.InvokeAsync(Opened);
    }
}