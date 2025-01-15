using FastRide.Server.Contracts;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
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

    [Function(SignalRConstants.ClientNotifyUserGeolocation)]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRMessageAction NotifyUserGeolocation(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientNotifyUserGeolocation, "userId",
            "groupName", "geolocation")]
        SignalRInvocationContext invocationContext, string userId, string groupName, Geolocation geolocation)
    {
        return new SignalRMessageAction(SignalRConstants.ServerNotifyUserGeolocation)
        {
            Arguments =
            [
                new NotifyUserGeolocation()
                {
                    UserId = userId,
                    Geolocation = geolocation
                }
            ],
            GroupName = groupName
        };
    }
}