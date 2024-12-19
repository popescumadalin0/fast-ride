using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastRide.Client.Authentication;

public class CustomUserFactory : AccountClaimsPrincipalFactory<CustomUserAccount>
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ISender _sender;

    private readonly IGeolocationService _geolocationService;

    public CustomUserFactory(IAccessTokenProviderAccessor accessor, IServiceProvider serviceProvider,
        ISignalRFactory signalRFactory, IGeolocationService geolocationService)
        : base(accessor)
    {
        _serviceProvider = serviceProvider;
        _geolocationService = geolocationService;
        _sender = signalRFactory.GetSenderAsync().GetAwaiter().GetResult();
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

            var city = await _geolocationService.GetLocationByLatLong(position.Latitude, position.Longitude);
            //todo: extract the correct city
            await _sender.JoinUserInGroupAsync($"{city}_{user.Response.UserType}");

            userIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Response.UserType.ToString()));
        }

        return initialUser;
    }
}