using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FastRide.Server.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    public string[] UserRoles { get; set; } = Array.Empty<string>();
}

/*
private bool UserHasRequiredPermissions(ClaimsPrincipal user)
    {
        var userPermissions = user.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToHashSet();

        return _requiredPermissions.All(rp => userPermissions.Contains(rp));
    }

    public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {


        var claimsPrincipal = authenticateResult.Principal;

        if (_requiredPermissions.Any() && !UserHasRequiredPermissions(claimsPrincipal))
        {
            var forbiddenResponse = req.CreateResponse(HttpStatusCode.Forbidden);
            await forbiddenResponse.WriteStringAsync("Forbidden: Insufficient permissions.");
            return forbiddenResponse;
        }

        return null;
    }
    */