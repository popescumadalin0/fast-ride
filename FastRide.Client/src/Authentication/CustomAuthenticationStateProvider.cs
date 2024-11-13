using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FastRide.Client.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
{
    private Task<AuthenticationState> _authenticationStateTask;
    
    /// <inheritdoc />
    public new void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
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