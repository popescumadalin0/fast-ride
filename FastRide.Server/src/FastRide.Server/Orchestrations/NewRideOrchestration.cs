using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Activities;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using Grpc.Core;
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
        context.SetCustomStatus(input);

        var priceCalculated = await PriceCalculationStepAsync(context, input);

        if (priceCalculated == 0)
        {
            _logger.LogInformation($"The ride was canceled!");
            await CancelWorkflow(context, input);

            return;
        }

        input.Cost = priceCalculated;
        context.SetCustomStatus(input);

        var paymentConfirmed = await PaymentStepAsync(context, input, priceCalculated);

        if (!paymentConfirmed)
        {
            _logger.LogInformation($"The ride was canceled!");
            await CancelWorkflow(context, input);
            return;
        }

        input.Status = InternRideStatus.NewRideAvailable;
        context.SetCustomStatus(input);

        var driverId = await FindDriverAsync(context, input.StartPoint, input.Destination, input.GroupName);

        if (string.IsNullOrEmpty(driverId))
        {
            await context.CallActivityAsync(
                nameof(CancelRideActivity),
                new CancelRideActivityInput()
                {
                    InstanceId = context.InstanceId,
                    UserId = input.User.NameIdentifier
                });

            _logger.LogInformation($"The ride was canceled!");
            await CancelWorkflow(context, input);

            return;
        }

        input.Driver = new UserIdentifier()
        {
            NameIdentifier = driverId
        };
        input.Status = InternRideStatus.GoingToUser;
        context.SetCustomStatus(input);

        await context.WaitForExternalEvent<bool>(SignalRConstants.ClientDriverArrived);
        input.Status = InternRideStatus.GoingToDestination;
        context.SetCustomStatus(input);

        await context.WaitForExternalEvent<bool>(SignalRConstants.ClientDriverArrived);
        await FinishWorkflow(context, input);
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
        var retries = 5;

        var excludeDriver = new List<string>();
        do
        {
            var driver = await context.CallActivityAsync<OnlineDriver>(nameof(FindDriverActivity),
                new FindDriverActivityInput()
                {
                    InstanceId = context.InstanceId,
                    UserGeolocation = userPosition,
                    Destination = userDestination,
                    GroupName = groupName,
                    ExcludeDrivers = excludeDriver
                });
            if (driver == null)
            {
                return string.Empty;
            }
            
            await context.CallActivityAsync(nameof(SendRideToDriverActivity), new SendRideToDriverActivityInput()
            {
                RequestInput = new FindDriverActivityInput()
                {
                    InstanceId = context.InstanceId,
                    UserGeolocation = userPosition,
                    Destination = userDestination,
                    GroupName = groupName,
                    ExcludeDrivers = excludeDriver
                },
                Driver = driver
            });
            
            var clientResponseTask =
                context.WaitForExternalEvent<DriverAcceptResponse>(SignalRConstants.ClientDriverAcceptRide);

            var timeoutTask = context.CallActivityAsync<DriverAcceptResponse>(nameof(DelayActivity),
                new DelayActivityInput()
                {
                    Seconds = 35,
                    DriverIdentifier = driver.Identifier.NameIdentifier,
                });

            var completedTask = await Task.WhenAny(clientResponseTask, timeoutTask);

            if (completedTask == clientResponseTask)
            {
                if ((await completedTask).Accepted)
                {
                    return (await completedTask).UserId;
                }

                excludeDriver.Add((await completedTask).UserId);
            }
            else
            {
                excludeDriver.Add((await timeoutTask).UserId);
            }
        } while (retries-- > 0);

        return string.Empty;
    }
    
    private static async Task FinishWorkflow(TaskOrchestrationContext context, NewRideInput input)
    {
        input.Status = InternRideStatus.Finished;
        context.SetCustomStatus(input);
        await context.CallActivityAsync(nameof(DelayActivity),
            new DelayActivityInput()
            {
                Seconds = 25
            });
        input.Status = InternRideStatus.None;
        context.SetCustomStatus(input);
        await context.CallActivityAsync(nameof(DelayActivity),
            new DelayActivityInput()
            {
                Seconds = 10
            });
    }

    private static async Task CancelWorkflow(TaskOrchestrationContext context, NewRideInput input)
    {
        input.Status = InternRideStatus.Cancelled;
        context.SetCustomStatus(input);
        await context.CallActivityAsync(nameof(DelayActivity),
            new DelayActivityInput()
            {
                Seconds = 25
            });
        input.Status = InternRideStatus.None;
        context.SetCustomStatus(input);
        await context.CallActivityAsync(nameof(DelayActivity),
            new DelayActivityInput()
            {
                Seconds = 10
            });
    }
}