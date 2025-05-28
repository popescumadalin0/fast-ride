using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Models;
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
            "userId", "accepted")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string instanceId,
        string userId,
        bool accepted)
    {
        _logger.LogInformation($"{nameof(DriverAcceptedRideTrigger)} function executed");

        await client.RaiseEventAsync(instanceId, SignalRConstants.ClientDriverAcceptRide, new DriverAcceptResponse()
        {
            UserId = invocationContext.UserId,
            Accepted = accepted
        });

        if (accepted)
        {
            return new SignalRMessageAction(SignalRConstants.ServerDriverRideAccepted)
            {
                Arguments =
                [
                    new UserIdentifier()
                    {
                        NameIdentifier = invocationContext.UserId,
                    }
                ],
                UserId = userId,
                GroupName = instanceId,
            };
        }

        return new SignalRMessageAction("nothing");
    }
}