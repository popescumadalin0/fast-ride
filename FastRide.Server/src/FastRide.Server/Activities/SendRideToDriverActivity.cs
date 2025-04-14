using System;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class SendRideToDriverActivity
{
    private readonly ILogger<SendRideToDriverActivity> _logger;

    private readonly IDistanceService _distanceService;

    public SendRideToDriverActivity(ILogger<SendRideToDriverActivity> logger,
        IDistanceService distanceService)
    {
        _logger = logger;
        _distanceService = distanceService;
    }

    [Function(nameof(SendRideToDriverActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRMessageAction Run([ActivityTrigger] SendRideToDriverActivityInput input)
    {
        _logger.LogInformation($"{nameof(SendRideToDriverActivity)} was triggered!");

        return new SignalRMessageAction(SignalRConstants.ServerDriverAcceptRide)
        {
            Arguments =
            [
                new DriverAcceptRide()
                {
                    InstanceId = input.RequestInput.InstanceId,
                    DestinationGeolocation = input.RequestInput.Destination,
                    Distance = _distanceService.GetDistanceBetweenLocations(input.RequestInput.UserGeolocation,
                        input.RequestInput.Destination)
                }
            ],
            UserId = input.Driver.Identifier.NameIdentifier
        };
    }
}