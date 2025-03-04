using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class DriverAcceptedRideTrigger
{
    private readonly ILogger<DriverAcceptedRideTrigger> _logger;

    public DriverAcceptedRideTrigger(ILogger<DriverAcceptedRideTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(DriverAcceptedRideTrigger))]
    public async Task<SignalRMessageAction> DriverAcceptedRide(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientDriverAcceptRide, "instanceId",
            "userId")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string instanceId,
        string userId)
    {
        _logger.LogInformation($"{nameof(DriverAcceptedRideTrigger)} function executed");

        await client.RaiseEventAsync(instanceId, SignalRConstants.ClientDriverAcceptRide, invocationContext.UserId);

        return new SignalRMessageAction(SignalRConstants.ServerDriverRideAccepted)
        {
            Arguments = [],
            UserId = userId,
        };
    }
}