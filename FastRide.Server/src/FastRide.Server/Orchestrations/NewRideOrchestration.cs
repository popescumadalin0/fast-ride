using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Activities;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
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

        var driverId = await FindDriverAsync(context, input.StartPoint, input.Destination, input.GroupName);

        if (!string.IsNullOrEmpty(driverId))
        {
            await context.CallActivityAsync(
                nameof(CancelRideActivity),
                new CancelRideActivityInput()
                {
                    InstanceId = context.InstanceId,
                    UserId = input.User.NameIdentifier
                });

            _logger.LogInformation($"The ride was canceled!");
            return;
        }

        //todo: when driver arrives to the user destination

        //todo: when user arrives at driver car

        //todo: when driver + users arrives at destination

        //todo: when user provide a note to the driver
    }

    private static async Task<double> PriceCalculationStepAsync(TaskOrchestrationContext context, NewRideInput input)
    {
        await context.CallActivityAsync(
            nameof(SendPriceCalculationActivity),
            new SendPriceCalculationActivityInput
            {
                InstanceId = context.InstanceId,
                UserId = input.User.NameIdentifier,
                Destination = input.Destination,
                StartPoint = input.StartPoint,
            });

        var doubleAccepted = await context.WaitForExternalEvent<string>(SignalRConstants.ClientSendPriceCalculation);
        var accepted = double.Parse(doubleAccepted);
        return accepted;
    }

    private static async Task<bool> PaymentStepAsync(TaskOrchestrationContext context, NewRideInput input, double price)
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

    private static async Task<string> FindDriverAsync(TaskOrchestrationContext context, Geolocation userPosition,
        Geolocation userDestination, string groupName)
    {
        var retries = 10;

        var excludeDriver = new List<string>();
        do
        {
            await context.CallActivityAsync(nameof(FindDriverActivity), new FindDriverActivityInput()
            {
                InstanceId = context.InstanceId,
                UserGeolocation = userPosition,
                Destination = userDestination,
                GroupName = groupName,
                ExcludeDrivers = excludeDriver
            });

            var accepted = await context.WaitForExternalEvent<string>(SignalRConstants.ClientDriverAcceptRide);

            if (!string.IsNullOrEmpty(accepted))
            {
                return accepted;
            }

            excludeDriver.Add(context.InstanceId);
        } while (retries-- > 0);

        return string.Empty;
    }
}