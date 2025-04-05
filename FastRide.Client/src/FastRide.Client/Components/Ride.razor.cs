using System.Threading.Tasks;
using FastRide.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Components;

public partial class Ride : ComponentBase
{
    [Parameter] public RideInformation RideInfo { get; set; }

    [Parameter] public EventCallback<RideInformation> OnRebook { get; set; }
    
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState{ get; set; }

}