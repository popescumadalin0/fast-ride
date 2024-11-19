using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Authentication;

public class HttpAuthenticationHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    public HttpAuthenticationHandler(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var token = await _accessTokenProvider.RequestAccessToken();

        if (token.Status != AccessTokenResultStatus.Success)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        if (!token.TryGetToken(out var accessToken))
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        return await base.SendAsync(request, cancellationToken);
    }
}