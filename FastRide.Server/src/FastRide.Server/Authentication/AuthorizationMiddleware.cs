using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace FastRide.Server.Authentication;

public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(
        FunctionContext context,
        FunctionExecutionDelegate next)
    {
        var targetMethod = context.GetTargetFunctionMethod();
        var customerAttributes = TokenRetriever.GetCustomAttributesOnClassAndMethod<AuthorizeAttribute>(targetMethod);
        if (!customerAttributes?.Any() ?? true)
        {
            await next(context);
            return;
        }

        if (!TokenRetriever.TryGetAccessToken(context, out var accessToken))
        {
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        var isValid = await IsAccessTokenValidAsync(accessToken);

        if (!isValid)
        {
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        await next(context);
    }

    private static async Task<bool> IsAccessTokenValidAsync(string accessToken)
    {
        using var client = new HttpClient();
        var response =
            await client.GetAsync(
                $"{Environment.GetEnvironmentVariable("Google:Api")}tokeninfo?access_token={accessToken}");

        return response.IsSuccessStatusCode;
    }
}