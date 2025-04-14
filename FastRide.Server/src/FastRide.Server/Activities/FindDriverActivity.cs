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

    private readonly IOnlineDriversService _onlineDriversService;

    public FindDriverActivity(ILogger<FindDriverActivity> logger, IOnlineDriversService onlineDriversService)
    {
        _logger = logger;
        _onlineDriversService = onlineDriversService;
    }

    [Function(nameof(FindDriverActivity))]
    public async Task<OnlineDriver> RunAsync([ActivityTrigger] FindDriverActivityInput input)
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

        return driver;
    }
}