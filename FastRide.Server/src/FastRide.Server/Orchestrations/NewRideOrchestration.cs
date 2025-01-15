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

       var acccepted = await PriceCalculationStep(context, input);

        if (!acccepted)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }
        
        if (!acccepted)
        {
            _logger.LogInformation($"The ride was canceled!"); 
            return;
        }
        
        //todo: search a rider
    }

    private async Task<bool> PriceCalculationStep(TaskOrchestrationContext context, NewRideInput input)
    {
        var response = await context.CallActivityAsync<Task<SignalRMessageAction>>(nameof(SendPriceCalculationActivity), new SendPriceCalculationActivityInput
        {
            Destination = input.Destination,
            InstanceId = context.InstanceId,
            StartPoint = input.StartPoint,
            UserId = input.User.NameIdentifier
        });
        
        var price = await response;
        
        var acccepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientSendPriceCalculation);
        
        return acccepted;
    }
    
    private async Task<bool> PaymentStep(TaskOrchestrationContext context, NewRideInput input)
    {
        await context.CallActivityAsync<object>(nameof(SendPaymentIntentActivity), new SendPaymentIntentActivityInput()
        {
            InstanceId = context.InstanceId,
            UserId = input.User.NameIdentifier,
            Price = input.Price,
        });
        
        var acccepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientPaymentConfirmation);
        
        return acccepted;
    }
}