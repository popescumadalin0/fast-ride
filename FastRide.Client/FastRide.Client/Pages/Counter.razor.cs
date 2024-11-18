using System.Threading.Tasks;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Pages;

public partial class Counter
{
    private int currentCount = 0;

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private IAccessTokenProvider TokenProvider { get; set; }

    private async Task IncrementCount()
    {
        var state = await TokenProvider.RequestAccessToken();
        state.TryGetToken(out var token);

        if (state.Status == AccessTokenResultStatus.Success)
        {
            currentCount = -10;
        }

        var a = token;
        var test = await FastRideApiClient.GetUserTypeAsync("test", "test");
        currentCount++;
    }
}