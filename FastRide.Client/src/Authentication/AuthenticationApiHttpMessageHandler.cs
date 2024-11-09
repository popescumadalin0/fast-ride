using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace FastRide_Client.Authentication;

/// <summary>
/// An <see cref="HttpMessageHandler"/> that configures the outgoing HTTP request to use the access token as bearer token.
/// </summary>
public class AuthenticationApiHttpMessageHandler : DelegatingHandler
{
    private readonly IBlazorServiceAccessor _blazorServiceAccessor;

    public AuthenticationApiHttpMessageHandler()
    {
    }

    public AuthenticationApiHttpMessageHandler(IBlazorServiceAccessor blazorServiceAccessor)
    {
        _blazorServiceAccessor = blazorServiceAccessor;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var sp = _blazorServiceAccessor.Services;
        var authenticationStateProvider = sp.GetRequiredService<AuthenticationStateProvider>();
        var memoryCache = sp.GetRequiredService<IMemoryCache>();

        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();

        if (memoryCache.TryGetValue($"access_token_{authState.User.Identity.Name}", out var token) != null)
        {
            var accessToken = token.ToString();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }

        var result = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            ReasonPhrase = "Valid access token not found, please sign-in again",
            Content = new StringContent("Valid access token not found, please sign-in again"),
            RequestMessage = new HttpRequestMessage()
        };
        return result;
    }
}