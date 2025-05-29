using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Activities;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Orchestrations;

public class NewRideOrchestration
{
    private readonly ILogger<NewRideOrchestration> _logger;

    private readonly IUserService _userService;

    public NewRideOrchestration(ILogger<NewRideOrchestration> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
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
            await CancelWorkflow(context, input, CompleteStatus.Cancelled);

            return;
        }

        input.Cost = priceCalculated;
        context.SetCustomStatus(input);

        var paymentConfirmed = await PaymentStepAsync(context, input, priceCalculated);

        if (!paymentConfirmed)
        {
            _logger.LogInformation($"The ride was canceled!");
            await CancelWorkflow(context, input, CompleteStatus.PaymentRefused);
            return;
        }

        input.Status = InternRideStatus.NewRideAvailable;
        context.SetCustomStatus(input);

        var driverId = await FindDriverAsync(context, input);

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
            await CancelWorkflow(context, input, CompleteStatus.DriverNotFound);

            return;
        }

        input.Driver = new UserIdentifier()
        {
            NameIdentifier = driverId
        };
        input.Status = InternRideStatus.GoingToUser;
        context.SetCustomStatus(input);

        Geolocation geolocation;
        const double tolerance = 0.0005;

        do
        {
            geolocation = await context.WaitForExternalEvent<Geolocation>(SignalRConstants.ClientDriverArrived);
        } while (Math.Abs(geolocation.Latitude - input.StartPoint.Latitude) > tolerance ||
                 Math.Abs(geolocation.Longitude - input.StartPoint.Longitude) > tolerance);

        input.Status = InternRideStatus.GoingToDestination;
        context.SetCustomStatus(input);

        do
        {
            geolocation = await context.WaitForExternalEvent<Geolocation>(SignalRConstants.ClientDriverArrived);
        } while (Math.Abs(geolocation.Latitude - input.Destination.Latitude) > tolerance ||
                 Math.Abs(geolocation.Longitude - input.Destination.Longitude) > tolerance);

        await FinishWorkflow(context, input);
    }

    private static async Task<double> PriceCalculationStepAsync(TaskOrchestrationContext context,
        NewRideInput input)
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

        var doubleAccepted =
            await context.WaitForExternalEvent<string>(SignalRConstants.ClientSendPriceCalculation);
        var accepted = double.Parse(doubleAccepted);
        return accepted;
    }

    private static async Task<bool> PaymentStepAsync(TaskOrchestrationContext context, NewRideInput input,
        double price)
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

    private static async Task<int> RatingStepAsync(TaskOrchestrationContext context, NewRideInput input)
    {
        await context.CallActivityAsync(nameof(SendRatingRequestActivity), new SendRatingRequestActivityInput()
        {
            InstanceId = context.InstanceId,
            UserId = input.User.NameIdentifier,
        });

        var rating = await context.WaitForExternalEvent<int>(SignalRConstants.ClientSendRatingRequest);

        return rating;
    }

    private static async Task<string> FindDriverAsync(TaskOrchestrationContext context, NewRideInput input)
    {
        var retries = 5;

        var excludeDriver = new List<string>();
        do
        {
            var driver = await context.CallActivityAsync<OnlineDriver>(nameof(FindDriverActivity),
                new FindDriverActivityInput()
                {
                    InstanceId = context.InstanceId,
                    UserGeolocation = input.StartPoint,
                    Destination = input.Destination,
                    GroupName = input.GroupName,
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
                    UserGeolocation = input.StartPoint,
                    Destination = input.Destination,
                    GroupName = input.GroupName,
                    ExcludeDrivers = excludeDriver,
                },
                Driver = driver,
                User = input.User
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
                await context.CallActivityAsync(nameof(NotifyDriverTimeoutActivity), (await timeoutTask).UserId);
            }
        } while (retries-- > 0);

        return string.Empty;
    }

    private async Task FinishWorkflow(TaskOrchestrationContext context, NewRideInput input)
    {
        input.Status = InternRideStatus.Finished;
        input.CompleteStatus = CompleteStatus.Completed;
        context.SetCustomStatus(input);
        await context.CallActivityAsync(nameof(DelayActivity),
            new DelayActivityInput()
            {
                Seconds = 25
            });

        var rating = await RatingStepAsync(context, input);

        if (rating != 0)
        {
            var response = await _userService.UpdateUserRatingAsync(input.Driver.NameIdentifier, rating);
            if (!response.Success)
            {
                _logger.LogError(response.ErrorMessage);
            }
        }

        input.Status = InternRideStatus.None;
        context.SetCustomStatus(input);
        await context.CallActivityAsync(nameof(DelayActivity),
            new DelayActivityInput()
            {
                Seconds = 35
            });
    }

    private static async Task CancelWorkflow(TaskOrchestrationContext context, NewRideInput input,
        CompleteStatus completeStatus)
    {
        input.Status = InternRideStatus.Cancelled;
        input.CompleteStatus = completeStatus;
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
                Seconds = 35
            });
    }
}