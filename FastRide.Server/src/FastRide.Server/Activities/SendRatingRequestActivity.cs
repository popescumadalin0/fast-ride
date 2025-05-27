using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Activities;

public class SendRatingRequestActivity
{
    private readonly ILogger<SendRatingRequestActivity> _logger;

    public SendRatingRequestActivity(ILogger<SendRatingRequestActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SendRatingRequestActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> RunAsync([ActivityTrigger] SendRatingRequestActivityInput input)
    {
        _logger.LogInformation($"{nameof(SendRatingRequestActivity)} was triggered!");

        return new SignalRMessageAction(SignalRConstants.ServerSendRatingRequest)
        {
            Arguments =
            [
                new RatingRequest()
                {
                    InstanceId = input.InstanceId,
                }
            ],
            UserId = input.UserId
        };
    }
}