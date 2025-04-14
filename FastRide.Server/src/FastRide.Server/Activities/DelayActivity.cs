using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class DelayActivity
{
    private readonly ILogger<DelayActivity> _logger;

    public DelayActivity(ILogger<DelayActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(DelayActivity))]
    public async Task<DriverAcceptResponse> RunAsync(
        [ActivityTrigger] DelayActivityInput input)
    {
        _logger.LogInformation($"DelayActivity triggered for: {input.Seconds}");
        await Task.Delay(input.Seconds);

        return new DriverAcceptResponse()
        {
            UserId = input.DriverIdentifier,
            Accepted = false
        };
    }
}