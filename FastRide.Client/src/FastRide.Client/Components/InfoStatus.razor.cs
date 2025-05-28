using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Components;

public partial class InfoStatus : IDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ICurrentRideState CurrentRideState { get; set; }
    
    public void Dispose()
    {
        CurrentRideState.OnChange -= CurrentRideStateOnChange;
    }

    protected override async Task OnInitializedAsync()
    {
        CurrentRideState.OnChange += CurrentRideStateOnChange;

        await base.OnInitializedAsync();
    }
    
    private Task CurrentRideStateOnChange()
    {
        StateHasChanged();
        
        return Task.CompletedTask;
    }
}