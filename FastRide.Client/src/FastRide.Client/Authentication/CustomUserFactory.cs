using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Server.Contracts;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FastRide.Client.Authentication;

public class CustomUserFactory : AccountClaimsPrincipalFactory<CustomUserAccount>
{
    private readonly IServiceProvider _serviceProvider;

    public CustomUserFactory(IAccessTokenProviderAccessor accessor, IServiceProvider serviceProvider)
        : base(accessor)
    {
        _serviceProvider = serviceProvider;
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(CustomUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var initialUser = await base.CreateUserAsync(account, options);

        if (initialUser?.Identity?.IsAuthenticated ?? false)
        {
            var userIdentity = (ClaimsIdentity)initialUser.Identity;

            var fastRideApiClient = _serviceProvider.GetRequiredService<IFastRideApiClient>();
            var userType = await fastRideApiClient.GetUserTypeAsync(
                new UserIdentifier()
                {
                    NameIdentifier =
                        initialUser.Claims.First(x => x.Type == "sub").Value,
                    Email =
                        initialUser.Claims.First(x => x.Type == "email").Value
                });

            if (!userType.Success)
            {
                throw new Exception($"Failed to get user roles: {userType.ResponseMessage}");
            }

            userIdentity.AddClaim(new Claim(ClaimTypes.Role, userType.Response.UserType.ToString()));
        }

        return initialUser;
    }
}