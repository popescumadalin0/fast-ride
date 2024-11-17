using System.Threading.Tasks;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class Counter
{
    private int currentCount = 0;

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    private async Task IncrementCount()
    {
        var test = await FastRideApiClient.GetUserTypeAsync("test", "test");

        currentCount++;
    }
}