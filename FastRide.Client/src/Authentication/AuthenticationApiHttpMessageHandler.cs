using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Constants;
using FastRide.Client.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FastRide.Client.Authentication;

/// <summary>
/// An <see cref="HttpMessageHandler"/> that configures the outgoing HTTP request to use the access token as bearer token.
/// </summary>
public class AuthenticationApiHttpMessageHandler : DelegatingHandler
{
    private readonly IBlazorServiceAccessor _blazorServiceAccessor;
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationApiHttpMessageHandler(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("id_token");
        
        /*var sp = _blazorServiceAccessor.Services;
        var jwtAccessor = sp.GetRequiredService<IJwtAccessor>();
        var accessToken = await jwtAccessor.ReadTokenAsync(TokenNames.AccessToken);*/
        request.Headers.Authorization =  new AuthenticationHeaderValue(_httpContextAccessor.HttpContext.Request.Scheme, _httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Identity.Application"]);

        var response = await base.SendAsync(request, cancellationToken);

        /*if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Refresh token and retry once.
            var tokens = await RefreshTokenAsync(jwtAccessor, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            // Retry.
            response = await base.SendAsync(request, cancellationToken);
        }
        */

        return response;
    }

    /*
    private async Task<LoginResponse> RefreshTokenAsync(IJwtAccessor jwtAccessor, CancellationToken cancellationToken)
    {
        // Get refresh token.
        var refreshToken = await jwtAccessor.ReadTokenAsync(TokenNames.RefreshToken);
        
        // Setup request to get new tokens.
        var url = remoteBackendAccessor.RemoteBackend.Url + "/identity/refresh-token";
        var refreshRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        refreshRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
        
        // Send request.
        var response = await base.SendAsync(refreshRequestMessage, cancellationToken);

        // If the refresh token is invalid, we can't do anything.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new LoginResponse(false, null, null);

        // Parse response into tokens.
        var tokens = (await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken))!;
        
        // Store tokens.
        await jwtAccessor.WriteTokenAsync(TokenNames.RefreshToken, tokens.RefreshToken!);
        await jwtAccessor.WriteTokenAsync(TokenNames.AccessToken, tokens.AccessToken!);
        
        // Return tokens.
        return tokens;
    }*/
}