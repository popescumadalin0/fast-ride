using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Layout;

public partial class NavMenu
{
    [Inject] private NavigationManager Navigation { get; set; }

    private void LogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
}