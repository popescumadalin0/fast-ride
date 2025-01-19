using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Stripe;

namespace FastRide.Server.Activities;

public class SendPaymentIntentActivity
{
    private readonly ILogger<SendPaymentIntentActivity> _logger;

    public SendPaymentIntentActivity(ILogger<SendPaymentIntentActivity> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SendPaymentIntentActivity))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> RunAsync([ActivityTrigger] SendPaymentIntentActivityInput input)
    {
        _logger.LogInformation($"{nameof(SendPaymentIntentActivity)} was triggered!");

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = (long)(input.Price * 100), // Amount in cents
            Currency = "$",
            PaymentMethodTypes = ["card"],
        });

        return new SignalRMessageAction(SignalRConstants.ServerSendPaymentIntent)
        {
            Arguments =
            [
                new SendPaymentIntent()
                {
                    InstanceId = input.InstanceId,
                    ClientSecret = paymentIntent.ClientSecret,
                }
            ],
            /*UserId = input.UserId*/
        };
    }
}