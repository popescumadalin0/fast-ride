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

        var priceCalculated = await PriceCalculationStep(context, input);

        if (!priceCalculated.Item1)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }
        
        var paymentConfirmed = await PaymentStep(context, input, priceCalculated.Item2);

        if (!paymentConfirmed)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }
        
        

        //todo: search a rider
    }

    private async Task<(bool, decimal)> PriceCalculationStep(TaskOrchestrationContext context, NewRideInput input)
    {
        var response = await context.CallActivityAsync<Task<SignalRMessageAction>>(nameof(SendPriceCalculationActivity),
            new SendPriceCalculationActivityInput
            {
                Destination = input.Destination,
                InstanceId = context.InstanceId,
                StartPoint = input.StartPoint,
                UserId = input.User.NameIdentifier
            });

        var price = await response;

        var accepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientSendPriceCalculation);

        return (accepted, (price.Arguments![0] as PriceCalculated)!.Price);
    }

    private async Task<bool> PaymentStep(TaskOrchestrationContext context, NewRideInput input, decimal price)
    {
        await context.CallActivityAsync<object>(nameof(SendPaymentIntentActivity), new SendPaymentIntentActivityInput()
        {
            InstanceId = context.InstanceId,
            UserId = input.User.NameIdentifier,
            Price = price,
        });

        var acccepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientPaymentConfirmation);

        return acccepted;
    }
}