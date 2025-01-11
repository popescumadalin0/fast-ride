using System.Threading.Tasks;
using FastRide.Server.Activities;
using FastRide.Server.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace FastRide.Server.Orchestrations;

public class StartNewRideOrchestration
{
    [Function(nameof(StartNewRideOrchestration))]
    public async Task RunOrchestratorAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<StartRideInput>();

        //todo:replace object with PaymentIndent and input with all details
        
        //todo: create price payment and send it to user for confirmation
        
        await context.CallActivityAsync<object>(nameof(PaymentActivity), "Tokyo");
    }
}