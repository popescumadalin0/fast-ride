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

public class FindDriverActivity
{
    private readonly ILogger<FindDriverActivity> _logger;

    private readonly IDistanceService _distanceService;

    private readonly IOnlineDriversService _onlineDriversService;

    public FindDriverActivity(ILogger<FindDriverActivity> logger, IOnlineDriversService onlineDriversService,
        IDistanceService distanceService)
    {
        _logger = logger; 
        _onlineDriversService = onlineDriversService;
        _distanceService = distanceService;
    }

    [Function(nameof(FindDriverActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> RunAsync([ActivityTrigger] FindDriverActivityInput input)
    {
        _logger.LogInformation($"{nameof(FindDriverActivity)} was triggered!");

        var driversResponse =
            await _onlineDriversService.GetClosestDriversByUserAsync(input.GroupName, input.UserGeolocation);

        if (!driversResponse.Success)
        {
            _logger.LogError(driversResponse.ErrorMessage);
            throw new Exception($"Unable to get closest driver for {input.GroupName}: {driversResponse.ErrorMessage}");
        }

        var drivers = driversResponse.Response;

        var driver = drivers.FirstOrDefault(d => !input.ExcludeDrivers.Contains(d.Identifier.NameIdentifier));

        if (driver != null)
        {
            return new SignalRMessageAction(SignalRConstants.ServerDriverAcceptRide)
            {
                Arguments =
                [
                    new DriverAcceptRide()
                    {
                        InstanceId = input.InstanceId,
                        DestinationGeolocation = input.Destination,
                        Distance = _distanceService.GetDistanceBetweenLocations(input.UserGeolocation,
                            input.Destination)
                    }
                ],
                UserId = driver.Identifier.NameIdentifier
            };
        }

        _logger.LogWarning($"Unable to find closest driver for {input.GroupName}! The ride is closed!");
        return new SignalRMessageAction(SignalRConstants.ServerCancelRide)
        {
            Arguments =
            [
                new CancelRide()
                {
                    InstanceId = input.InstanceId,
                }
            ],
            GroupName = input.GroupName
        };
    }
}