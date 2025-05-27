using System;
using System.Threading.Tasks;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private OverlayState OverlayState { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }
    
    private bool _developmentMode;
    
    public void Dispose()
    {
        OverlayState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        OverlayState.OnChange += StateHasChanged;

#if DEBUG
        _developmentMode = true;
#endif

        await base.OnInitializedAsync();
    }
}