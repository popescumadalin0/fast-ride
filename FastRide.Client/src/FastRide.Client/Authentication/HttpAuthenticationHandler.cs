using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace FastRide.Client.Authentication;

public class HttpAuthenticationHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    private readonly ISessionStorageService _sessionStorage;

    private readonly IConfiguration _configuration;

    public HttpAuthenticationHandler(IAccessTokenProvider accessTokenProvider, ISessionStorageService sessionStorage,
        IConfiguration configuration)
    {
        _accessTokenProvider = accessTokenProvider;
        _sessionStorage = sessionStorage;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var accessTokenResponse = await _accessTokenProvider.RequestAccessToken();

        var baseUri = _configuration["Google:Authority"];
        var clientId = _configuration["Google:ClientId"] ??
                       throw new ArgumentNullException($"{_configuration["Google:ClientId"]}");
        var key = $"oidc.user:{baseUri}:{clientId}";

        var tokenId = _sessionStorage.GetItem<TokenSession>(key).id_token;

        if (accessTokenResponse.Status != AccessTokenResultStatus.Success)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        if (!accessTokenResponse.TryGetToken(out var accessToken))
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        request.Headers.Add("Authentication", $"Bearer {tokenId}");
        request.Headers.Add("Authorization", $"Bearer {accessToken.Value}");

        return await base.SendAsync(request, cancellationToken);
    }
}