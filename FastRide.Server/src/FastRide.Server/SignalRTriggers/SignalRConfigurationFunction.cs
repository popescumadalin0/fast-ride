using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class SignalRConfigurationFunction
{
    private readonly ILogger<SignalRConfigurationFunction> _logger;

    public SignalRConfigurationFunction(ILogger<SignalRConfigurationFunction> logger)
    {
        _logger = logger;
    }

    [Function("negotiate")]
    public IActionResult Negotiate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "negotiate")]
        HttpRequest req,
        [SignalRConnectionInfoInput(HubName = "serverless")]
        SignalRConnectionInfo connectionInfo)
    {
        _logger.LogInformation("Negotiate request received");
        return new ObjectResult(connectionInfo);
    }
}