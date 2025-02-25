using System.Collections.Generic;
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

        if (priceCalculated == 0)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }

        var paymentConfirmed = await PaymentStepAsync(context, input, priceCalculated);

        if (!paymentConfirmed)
        {
            _logger.LogInformation($"The ride was canceled!");
            return;
        }

        //todo: search a rider

        var driverFound = FindDriver(context, input.StartPoint, input.Destination);


    }

    private async Task<decimal> PriceCalculationStepAsync(TaskOrchestrationContext context, NewRideInput input)
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
        
        var accepted = await context.WaitForExternalEvent<decimal>(SignalRConstants.ClientSendPriceCalculation);

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

    private async Task<bool> FindDriver(TaskOrchestrationContext context, Geolocation userPosition,
        Geolocation userDestination)
    {
        var retries = 10;
        //todo: if driver refuse the ride
        var excludeDriver = new List<string>();
        do
        {
            await context.CallActivityAsync(nameof(FindDriverActivity), new FindDriverActivityInput()
            {
                InstanceId = context.InstanceId,
                UserGeolocation = userPosition,
                Destination = userDestination,
            });

            var acccepted = await context.WaitForExternalEvent<bool>(SignalRConstants.ClientDriverAcceptRide);

            if (acccepted)
            {
                return true;
            }

            excludeDriver.Add(context.InstanceId);
        }
        while(retries-- > 0);

        return false;
    }
}