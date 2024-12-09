using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Contracts;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using UserType = FastRide.Server.Services.Enums.UserType;

namespace FastRide.Server.Authentication;

public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IUserService _userService;

    public AuthenticationMiddleware(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Invoke(
        FunctionContext context,
        FunctionExecutionDelegate next)
    {
        var targetMethod = context.GetTargetFunctionMethod();
        var customerAttributes = TokenRetriever.GetCustomAttributesOnClassAndMethod<AuthorizeAttribute>(targetMethod);

        var roles = customerAttributes.FirstOrDefault()?.UserRoles;

        if (!TokenRetriever.TryGetIdToken(context, out var idToken))
        {
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string>() { Environment.GetEnvironmentVariable("Google:ClientId") }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            var principal = ConvertPayloadToClaimsPrincipal(payload);
            ((DefaultHttpContext)context.Items["HttpRequestContext"]).User = principal;
            
            if (roles is null || roles.Length == 0)
            {
                await next(context);
                return;
            }
            
            var email = payload.Email;
            var nameIdentifier = payload.Subject;

            var user = await _userService.GetUserAsync(new UserIdentifier()
            {
                Email = email,
                NameIdentifier = nameIdentifier
            });

            if (!user.Success)
            {
                context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                return;
            }

            if (!roles.Contains((UserType)user.Response.UserType))
            {
                context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                return;
            }
        }
        catch (Exception e)
        {
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        await next(context);
    }

    private static ClaimsPrincipal ConvertPayloadToClaimsPrincipal(GoogleJsonWebSignature.Payload payload)
    {
        var claims = new List<Claim>
        {
            new("sub", payload.Subject),
            new("name", payload.Name),
            new("email", payload.Email),
        };

        if (!string.IsNullOrEmpty(payload.Picture))
        {
            claims.Add(new Claim("picture", payload.Picture));
        }

        if (!string.IsNullOrEmpty(payload.Issuer))
        {
            claims.Add(new Claim(ClaimTypes.GroupSid, payload.Issuer));
        }

        var identity = new ClaimsIdentity(claims, "Google");

        return new ClaimsPrincipal(identity);
    }
}