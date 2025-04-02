using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Orchestrations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class CreateNewRideTrigger
{
    private readonly ILogger<CreateNewRideTrigger> _logger;

    public CreateNewRideTrigger(ILogger<CreateNewRideTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(CreateNewRideTrigger))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> CreateRide(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientCreateNewRide, "ride")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        NewRideInput ride)
    {
        var instance = await client.ScheduleNewOrchestrationInstanceAsync(nameof(NewRideOrchestration), input: ride);

        return new SignalRMessageAction(SignalRConstants.ServerCreateNewRide)
        {
            Arguments =
            [
                new RideCreated() { InstanceId = instance }
            ],
            UserId = invocationContext.UserId,
        };
    }
}