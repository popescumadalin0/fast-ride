using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Contracts;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

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
            
            if (roles is null || roles.Length == 0)
            {
                //context.GetHttpContext().SignInAsync()
                await next(context);
                return;
            }
            
            var email = payload.Email;
            var nameIdentifier = payload.Subject;

            var userType = await _userService.GetUserType(new UserIdentifier()
            {
                Email = email,
                NameIdentifier = nameIdentifier
            });

            if (!userType.Success)
            {
                context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                return;
            }

            if (!roles.Contains(userType.Response.ToString()))
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
}