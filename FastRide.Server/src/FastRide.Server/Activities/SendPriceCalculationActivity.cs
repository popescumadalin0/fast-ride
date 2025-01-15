using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class SendPriceCalculationActivity
{
    private readonly ILogger<SendPriceCalculationActivity> _logger;

    private readonly IDistanceService _distanceService;

    public SendPriceCalculationActivity(ILogger<SendPriceCalculationActivity> logger, IDistanceService distanceService)
    {
        _logger = logger;
        _distanceService = distanceService;
    }

    [Function(nameof(SendPriceCalculationActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public Task<SignalRMessageAction> RunAsync(
        [ActivityTrigger] SendPriceCalculationActivityInput input)
    {
        _logger.LogInformation("Saying hello to {name}.", input);

        var distance = 0;
        var duration = 0;
        //todo: using google, calculate the distance and the duration
        var price = _distanceService.CalculatePricePerDistance(distance, duration);

        return Task.FromResult(new SignalRMessageAction(SignalRConstants.ServerSendPriceCalculation)
        {
            Arguments =
            [
                new PriceCalculated()
                {
                    Price = price,
                    InstanceId = input.InstanceId,
                }
            ],
            /*UserId = input.UserId*/
        });
    }
}