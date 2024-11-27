using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class Ride : ComponentBase
{
    [Parameter] public string Action { get; set; }
}