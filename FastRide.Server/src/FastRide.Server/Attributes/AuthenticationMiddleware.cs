using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace FastRide.Server.Attributes;

public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationMiddleware(IConfiguration configuration, IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task Invoke(
        FunctionContext context,
        FunctionExecutionDelegate next)
    {
        if (!TryGetPrincipalFromCookies(context, out var principal))
        {
            // Unable to get token from headers
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        /*
        if (!_tokenValidator.CanReadToken(token))
        {
            // Token is malformed
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        // Get OpenID Connect metadata
        var validationParameters = _tokenValidationParameters.Clone();
        var openIdConfig = await _configurationManager.GetConfigurationAsync(default);
        validationParameters.ValidIssuer = openIdConfig.Issuer;
        validationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;*/

        /*try
        {
            // Validate token
            var principal = _tokenValidator.ValidateToken(
                token, validationParameters, out _);

            // Set principal + token in Features collection
            // They can be accessed from here later in the call chain
            context.Features.Set(new JwtPrincipalFeature(principal, token));

            await next(context);
        }
        catch (SecurityTokenException)
        {
            // Token is not valid (expired etc.)
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }*/
    }

    private static bool TryGetPrincipalFromCookies(FunctionContext context, out string token)
    {
        token = null;
        // HTTP headers are in the binding context as a JSON object
        // The first checks ensure that we have the JSON string
        if (!context.BindingContext.BindingData.TryGetValue("Cockies", out var cookies))
        {
            return false;
        }

        if (cookies is not string cookieString)
        {
            return false;
        }

        // Deserialize headers from JSON
        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(cookieString);
        var normalizedKeyHeaders = headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value);
        if (!normalizedKeyHeaders.TryGetValue("authorization", out var authHeaderValue))
        {
            // No Authorization header present
            return false;
        }

        if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            // Scheme is not Bearer
            return false;
        }
        
        /*var authenticateResult =
            await _authenticationService.AuthenticateAsync(httpContext,
                CookieAuthenticationDefaults.AuthenticationScheme);*/

        token = authHeaderValue.Substring("Bearer ".Length).Trim();
        return true;
    }
}