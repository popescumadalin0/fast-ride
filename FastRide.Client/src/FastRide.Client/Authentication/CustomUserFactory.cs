﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Models;
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
            var user = await fastRideApiClient.GetCurrentUserAsync();

            if (!user.Success)
            {
                return new ClaimsPrincipal();
            }

            userIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Response.UserType.ToString()));
            if (
                userIdentity.TryRemoveClaim(userIdentity.Claims.SingleOrDefault(x => x.Type == "sub"))
            )
            {
                userIdentity.AddClaim(new Claim("sub", user.Response.Identifier.NameIdentifier));
            }
        }

        return initialUser;
    }
}