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
    private readonly ISender _sender;
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserGroupService _userGroupService;

    public CustomUserFactory(IAccessTokenProviderAccessor accessor, IServiceProvider serviceProvider,
        ISender sender, IUserGroupService userGroupService)
        : base(accessor)
    {
        _serviceProvider = serviceProvider;
        _userGroupService = userGroupService;
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

            var userGroup = await _userGroupService.GetCurrentUserGroupNameAsync();

            var userId = user.Response.Identifier.NameIdentifier;

            await _sender.JoinUserInGroupAsync(userId, userGroup);

            userIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Response.UserType.ToString()));
        }

        return initialUser;
    }
}