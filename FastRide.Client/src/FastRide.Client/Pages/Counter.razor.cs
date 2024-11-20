using System.Threading.Tasks;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.Pages;

public partial class Counter
{
    private int currentCount = 0;

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }

    
    private async Task IncrementCount()
    {
        var stateUser = await AuthenticationState;
        //var test = await FastRideApiClient.GetUserTypeAsync("test", "test");
        //var a = test.Response.UserType; 
        currentCount++;
    }
}