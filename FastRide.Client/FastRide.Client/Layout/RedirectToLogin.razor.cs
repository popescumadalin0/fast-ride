using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Layout;

public partial class RedirectToLogin : ComponentBase
{
    protected override void OnInitialized()
    {
        Navigation.NavigateToLogin("authentication/login");
    }
}