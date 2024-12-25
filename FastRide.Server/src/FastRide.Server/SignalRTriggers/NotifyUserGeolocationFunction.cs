using FastRide.Server.Contracts;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class NotifyUserGeolocationFunction
{
    private readonly ILogger<NotifyUserGeolocationFunction> _logger;

    public NotifyUserGeolocationFunction(ILogger<NotifyUserGeolocationFunction> logger)
    {
        _logger = logger;
    }

    [Function(SignalRConstants.NotifyUserGeolocation)]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRMessageAction JoinUserToGroup(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.NotifyUserGeolocation, "userId",
            "groupName", "geolocation")]
        SignalRInvocationContext invocationContext, string userId, string groupName, Geolocation geolocation)
    {
        return new SignalRMessageAction("clientMessage")
        {
            Arguments = invocationContext.Arguments,
            GroupName = groupName
        };
    }
}