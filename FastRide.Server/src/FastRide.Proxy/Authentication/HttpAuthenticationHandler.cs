using System.Net;

namespace FastRide.Proxy.Authentication;

public class HttpAuthenticationHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;

    public HttpAuthenticationHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Headers.Add("x-functions-key", _configuration.GetValue<string>("Server:Key"));

        return await base.SendAsync(request, cancellationToken);
    }
}