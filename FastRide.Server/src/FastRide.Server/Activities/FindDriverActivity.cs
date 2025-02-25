using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stripe;

namespace FastRide.Server.Activities;

public class FindDriverActivity
{
    private readonly ILogger<FindDriverActivity> _logger;

    private readonly IOnlineDriversService _onlineDriversService;

    public FindDriverActivity(ILogger<FindDriverActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FindDriverActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> RunAsync([ActivityTrigger] FindDriverActivityInput input)
    {
        _logger.LogInformation($"{nameof(FindDriverActivity)} was triggered!");

        var driver = await _onlineDriversService.GetOnlineDriversByGroupName(input.GroupName);

        return new SignalRMessageAction(SignalRConstants.ServerDriverAcceptRide)
        {
            Arguments =
            [
                new DriverAcceptRide()
                {
                    InstanceId = input.InstanceId,
                    DestinationGeolocation = input.Destination,
                    UserGeolocation = input.UserGeolocation,
                }
            ],
            /*UserId = input.UserId*/
        };
    }
}