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

        var priceCalculated = await PriceCalculationStepAsync(context, input);

        if (!priceCalculated)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }

        var paymentConfirmed = await PaymentStepAsync(context, input, priceCalculated.Item2);

        if (!paymentConfirmed)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }


        //todo: search a rider
    }

    private async Task<bool> PriceCalculationStepAsync(TaskOrchestrationContext context, NewRideInput input)
    {
        await context.CallActivityAsync(
            nameof(SendPriceCalculationActivity),
            new SendPriceCalculationActivityInput
            {
                Destination = input.Destination,
                InstanceId = context.InstanceId,
                StartPoint = input.StartPoint,
                UserId = input.User.NameIdentifier
            });
        
        var accepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientSendPriceCalculation);

        return accepted;
    }

    private async Task<bool> PaymentStepAsync(TaskOrchestrationContext context, NewRideInput input, decimal price)
    {
        await context.CallActivityAsync(nameof(SendPaymentIntentActivity), new SendPaymentIntentActivityInput()
        {
            InstanceId = context.InstanceId,
            UserId = input.User.NameIdentifier,
            Price = price,
        });

        var acccepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientSendPaymentIntent);

        return acccepted;
    }
}