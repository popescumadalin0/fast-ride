using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide_Client.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider,
    IHostEnvironmentAuthenticationStateProvider
{
    private Task<AuthenticationState> _authenticationStateTask;

    /// <inheritdoc />
    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask =
            authenticationStateTask ?? throw new ArgumentNullException(nameof(authenticationStateTask));
        NotifyAuthenticationStateChanged(_authenticationStateTask);
    }

    /// <inheritdoc />
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => _authenticationStateTask
           ?? throw new InvalidOperationException(
               $"Do not call {nameof(GetAuthenticationStateAsync)} outside of the DI scope for a Razor component. Typically, this means you can call it only within a Razor component or inside another DI service that is resolved for a Razor component.");

    public void SignOut()
    {
        _authenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        NotifyAuthenticationStateChanged(_authenticationStateTask);
    }
}