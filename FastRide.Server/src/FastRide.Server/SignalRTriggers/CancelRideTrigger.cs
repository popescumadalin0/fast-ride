using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FastRide.Server.SignalRTriggers;

public class CancelRideTrigger
{
    private readonly ILogger<CancelRideTrigger> _logger;

    public CancelRideTrigger(ILogger<CancelRideTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(CancelRideTrigger))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> CancelRide(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientCancelRide)]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client)
    {
        var instances = client.GetAllInstancesAsync();

        await foreach (var instance in instances)
        {
            if (instance.RuntimeStatus is OrchestrationRuntimeStatus.Running or OrchestrationRuntimeStatus.Suspended)
            {
                await client.TerminateInstanceAsync(instance.InstanceId);

                return new SignalRMessageAction(SignalRConstants.ServerCancelRide)
                {
                    GroupName = instance.InstanceId,
                };
            }
        }

        return new SignalRMessageAction("nothing");
    }
}