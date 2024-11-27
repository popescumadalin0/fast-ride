using FastRide.Client.Models;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Components;

public partial class Ride : ComponentBase
{
    [Parameter] public RideInformation RideInfo { get; set; }
}