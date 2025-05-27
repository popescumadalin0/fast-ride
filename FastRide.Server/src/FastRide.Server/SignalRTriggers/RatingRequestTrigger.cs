using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class RatingRequestTrigger
{
    private readonly ILogger<RatingRequestTrigger> _logger;

    public RatingRequestTrigger(ILogger<RatingRequestTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(RatingRequestTrigger))]
    public async Task DriverArrived(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientSendRatingRequest, "groupName",
            "rating")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string groupName,
        int rating)
    {
        _logger.LogInformation($"{nameof(SendPriceCalculationTrigger)} function executed");

        await client.RaiseEventAsync(groupName, SignalRConstants.ClientSendRatingRequest, rating);
    }
}