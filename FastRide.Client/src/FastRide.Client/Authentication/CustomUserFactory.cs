using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FastRide.Client.Authentication;

public class CustomUserFactory : AccountClaimsPrincipalFactory<CustomUserAccount>
{
    private readonly IGeolocationService _geolocationService;

    private readonly ISender _sender;
    private readonly IServiceProvider _serviceProvider;

    public CustomUserFactory(IAccessTokenProviderAccessor accessor, IServiceProvider serviceProvider,
        ISender sender, IGeolocationService geolocationService)
        : base(accessor)
    {
        _serviceProvider = serviceProvider;
        _geolocationService = geolocationService;
        _sender = sender;
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
                throw new Exception($"Failed to get user roles: {user.ResponseMessage}");
            }

            var position = await _geolocationService.GetLocationAsync();

            var city = await _geolocationService.GetLocalityByLatLongAsync(position.Latitude, position.Longitude);
            var userGroup = $"{city}_{user.Response.UserType}";
            var userId = user.Response.Identifier.NameIdentifier;

            await _sender.JoinUserInGroupAsync(userId, userGroup);

            userIdentity.AddClaim(new Claim(ClaimTypes.GroupSid, userGroup));
            userIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Response.UserType.ToString()));
        }

        return initialUser;
    }
}