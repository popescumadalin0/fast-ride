using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class NotifyUserGeolocationTrigger
{
    private readonly ILogger<NotifyUserGeolocationTrigger> _logger;

    public NotifyUserGeolocationTrigger(ILogger<NotifyUserGeolocationTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(NotifyUserGeolocationTrigger))]
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
            GroupName = groupName,
        };
    }
}