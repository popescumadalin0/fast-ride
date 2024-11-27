using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace FastRide.Client.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; }
    
    [Inject] private DestinationState DestinationState { get; set; }
    
    protected override void OnInitialized()
    {
        DestinationState.OnChange += StateHasChanged;
        base.OnInitialized();
    }
    
    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }
    
    private async Task DriveAsync()
    {
        
    }

    private void LogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
}