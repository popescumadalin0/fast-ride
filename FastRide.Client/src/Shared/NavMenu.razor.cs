using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace FastRide.Client.Shared;

public partial class NavMenu
{
    private bool collapseNavMenu = true;
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private AuthenticationStateProvider AuthProvider { get; set; }

    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }

    private bool isAuth { get; set; }
    private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var auth = await AuthProvider.GetAuthenticationStateAsync();
            isAuth = auth.User.Identity?.IsAuthenticated ?? false;
        }


        await base.OnAfterRenderAsync(firstRender);
    }

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }

    private void LoginWithGoogle()
    {
        var returnUrl = Navigation.ToBaseRelativePath(Navigation.Uri);
        Navigation.NavigateTo($"login-google?returnUrl={returnUrl}", true);
    }

    private async Task LogoutAsync()
    {
    }
}