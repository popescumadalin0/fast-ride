using System;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Components;

public partial class AcceptAvailableRidePopup : IDisposable
{
    [Parameter] public bool Opened { get; set; }

    [Parameter] public EventCallback<bool> OpenedChanged { get; set; }

    [Parameter] public RideInformation AvailableRide { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }


    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;

        await base.OnInitializedAsync();
    }

    private async Task AcceptRideAsync(RideInformation ride)
    {
        Opened = false;
        await OpenedChanged.InvokeAsync(Opened);
    }
}