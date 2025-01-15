using System.Threading.Tasks;
using FastRide.Server.Activities;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Orchestrations;

public class NewRideOrchestration
{
    private readonly ILogger<NewRideOrchestration> _logger;

    public NewRideOrchestration(ILogger<NewRideOrchestration> logger)
    {
        _logger = logger;
    }

    [Function(nameof(NewRideOrchestration))]
    public async Task RunOrchestratorAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        _logger.LogInformation($"{nameof(NewRideOrchestration)} was triggered!");

        var input = context.GetInput<NewRideInput>();

        await context.CallActivityAsync<object>(nameof(SendPriceCalculationActivity), new SendPriceCalculationActivityInput
        {
           Destination = input.Destination,
           InstanceId = context.InstanceId,
           StartPoint = input.StartPoint,
           UserId = input.User.NameIdentifier
        });
        
        var acccepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientSendPriceCalculation);

        if (!acccepted)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }
        
        //todo:replace object with PaymentIndent and input with all details
        //todo: create price payment and send it to user for confirmation
        await context.CallActivityAsync<object>(nameof(PaymentActivity), "Tokyo");

        // = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientSendPayment);
        
        if (!acccepted)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }
        
        //todo: search a rider
    }
}