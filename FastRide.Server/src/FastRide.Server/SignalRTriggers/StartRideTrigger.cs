using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Orchestrations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class StartRideTrigger
{
    private readonly ILogger<StartRideTrigger> _logger;

    public StartRideTrigger(ILogger<StartRideTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(StartRideTrigger))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> StartRide(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.StartRide, "groupName", "ride")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string groupName,
        StartRide ride)
    {
        var instance = await client.ScheduleNewOrchestrationInstanceAsync(nameof(StartNewRideOrchestration), input: ride);

        return new SignalRMessageAction(SignalRConstants.RideCreated)
        {
            Arguments =
            [
                new { InstanceId = instance }
            ],
            GroupName = groupName,
            UserId = invocationContext.UserId,
        };
    }
}