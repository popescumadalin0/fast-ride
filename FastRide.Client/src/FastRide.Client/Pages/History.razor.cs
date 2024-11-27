using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class History
{
    [Inject]
    private IFastRideApiClient FastRideApiClient { get; set; }
    
    private List<RideInformation> _rideGroups;

    protected override async Task OnInitializedAsync()
    {
        var response = await FastRideApiClient.GetRidesByUserAsync();
    }
}