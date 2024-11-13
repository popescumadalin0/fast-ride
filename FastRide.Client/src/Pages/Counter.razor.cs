using System.Threading.Tasks;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace FastRide.Client.Pages;

public partial class Counter
{
    [Inject]
    private IFastRideApiClient FastRideApiClient { get; set; }
    
    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; }
    
    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    
    private int currentCount = 0;

    private async Task IncrementCount()
    {
        var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var currentUser = state.User;
        var token = await HttpContextAccessor.HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme,"id_token");
        
        var test = await FastRideApiClient.GetUserTypeAsync("test", "test");
        currentCount++;
    }
}