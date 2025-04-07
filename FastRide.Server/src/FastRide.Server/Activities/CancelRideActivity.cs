using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class CancelRideActivity
{
    private readonly ILogger<CancelRideActivity> _logger;

    public CancelRideActivity(ILogger<CancelRideActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(CancelRideActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public Task<SignalRMessageAction> RunAsync(
        [ActivityTrigger] CancelRideActivityInput input)
    {
        _logger.LogInformation("Saying hello to {name}.", input);

        return Task.FromResult(new SignalRMessageAction(SignalRConstants.ServerCancelRide)
        {
            UserId = input.InstanceId
        });
    }
}