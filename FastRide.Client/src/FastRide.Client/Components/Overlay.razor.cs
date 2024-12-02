using System;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Components;

public partial class Overlay : ComponentBase, IDisposable
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