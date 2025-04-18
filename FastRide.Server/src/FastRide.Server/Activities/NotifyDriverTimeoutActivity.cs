using FastRide.Server.Contracts.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class NotifyDriverTimeoutActivity
{
    private readonly ILogger<NotifyDriverTimeoutActivity> _logger;

    public NotifyDriverTimeoutActivity(ILogger<NotifyDriverTimeoutActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(NotifyDriverTimeoutActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRMessageAction Run([ActivityTrigger] string driverId)
    {
        _logger.LogInformation($"{nameof(NotifyDriverTimeoutActivity)} was triggered!");

        return new SignalRMessageAction(SignalRConstants.ServerNotifyDriverTimeout)
        {
            Arguments = [],
            UserId = driverId
        };
    }
}