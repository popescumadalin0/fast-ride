using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class SaveRideInTableActivity
{
    private readonly ILogger<SaveRideInTableActivity> _logger;

    private readonly IRideService _rideService;

    public SaveRideInTableActivity(ILogger<SaveRideInTableActivity> logger, IRideService rideService)
    {
        _logger = logger;
        _rideService = rideService;
    }

    [Function(nameof(SaveRideInTableActivity))]
    public async Task RunAsync(
        [ActivityTrigger] Ride input)
    {
        _logger.LogInformation($"{nameof(SaveRideInTableActivity)} started!");

        var serviceResponse = await _rideService.AddRideAsync(input);

        /*UserId = input.UserId*/
    }
}