using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Layout;

public partial class LoginDisplay : ComponentBase
{
    public void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
}