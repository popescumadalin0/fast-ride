using System;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Layout;

public partial class MainLayout : IDisposable
{
    [Inject]
    private OverlayState OverlayState { get; set; }

    protected override void OnInitialized()
    {
        OverlayState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        OverlayState.OnChange -= StateHasChanged;
    }
}