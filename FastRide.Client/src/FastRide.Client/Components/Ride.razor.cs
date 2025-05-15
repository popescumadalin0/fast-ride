using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Components;

public partial class Ride : ComponentBase
{
    [Parameter] public RideInformation RideInfo { get; set; }

    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; }
    
    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    private async Task RebookClickedAsync()
    {
        DestinationState.Geolocation = RideInfo.DestinationLocation;
        NavigationManager.NavigateTo("/");
    }
}