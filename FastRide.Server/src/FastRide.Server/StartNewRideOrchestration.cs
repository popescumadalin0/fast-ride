using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace FastRide.Server;

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