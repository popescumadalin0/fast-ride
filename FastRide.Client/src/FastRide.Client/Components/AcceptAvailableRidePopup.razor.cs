using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Client.State;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class AcceptAvailableRidePopup : IDisposable
{
    [Parameter] public bool Opened { get; set; }

    [Parameter] public EventCallback<bool> OpenedChanged { get; set; }

    [Parameter] public RideInformation AvailableRide { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }
    

    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;

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