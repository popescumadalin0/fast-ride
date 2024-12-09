using System.Threading.Tasks;
using FastRide.Server.Activities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace FastRide.Server.Orchestrations;

public class StartNewRideOrchestration
{
    [Function(nameof(StartNewRideOrchestration))]
    public async Task RunOrchestratorAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        //todo:replace object with PaymentIndent and input with all details
        await context.CallActivityAsync<object>(nameof(PaymentActivity), "Tokyo");
    }
}