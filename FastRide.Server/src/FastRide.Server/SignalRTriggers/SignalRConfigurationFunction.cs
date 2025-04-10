using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class SignalRConfigurationFunction
{
    private readonly ILogger<SignalRConfigurationFunction> _logger;

    private readonly IOnlineDriversService _driversService;

    public SignalRConfigurationFunction(ILogger<SignalRConfigurationFunction> logger,
        IOnlineDriversService driversService)
    {
        _logger = logger;
        _driversService = driversService;
    }

    [Function("negotiate")]
    public IActionResult Negotiate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "negotiate")]
        HttpRequest req,
        [SignalRConnectionInfoInput(HubName = SignalRConstants.HubName, UserId = "{query.userId}")]
        SignalRConnectionInfo signalRConnectionInfo)
    {
        _logger.LogInformation("Negotiate request received");

        return new ObjectResult(signalRConnectionInfo);
    }

    [Function("onconnected")]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessage> OnConnected(
        [SignalRTrigger(hubName: SignalRConstants.HubName, category: "connections", @event: "connected")]
        SignalRInvocationContext context)
    {
        return new SignalRMessage
        {
            UserId = context.UserId,
        };
    }

    [Function("ondisconnected")]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessage> OnDisconnected(
        [SignalRTrigger(hubName: SignalRConstants.HubName, category: "connections", @event: "disconnected")]
        SignalRInvocationContext context)
    {
        await _driversService.DeleteOnlineDriverAsync(context.UserId);

        return new SignalRMessage
        {
            UserId = context.UserId,
        };
    }
}