using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class Authentication : ComponentBase
{
    [Parameter] public string Action { get; set; }
}