using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Layout;

public partial class InfoStatus : IDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }


    [Inject] private CurrentRideState CurrentRideState { get; set; }

    public void Dispose()
    {
        CurrentRideState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        CurrentRideState.OnChange += StateHasChanged;

        await base.OnInitializedAsync();
    }
}