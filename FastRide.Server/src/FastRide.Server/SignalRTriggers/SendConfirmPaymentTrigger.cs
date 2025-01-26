using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class SendConfirmPaymentTrigger
{
    private readonly ILogger<SendConfirmPaymentTrigger> _logger;

    public SendConfirmPaymentTrigger(ILogger<SendConfirmPaymentTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SendConfirmPaymentTrigger))]
    public async Task SendPaymentConfirm(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientSendPaymentIntent, "instanceId",
            "paymentSuccess")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string instanceId,
        bool paymentSuccess)
    {
        _logger.LogInformation($"{nameof(SendConfirmPaymentTrigger)} function executed");

        await client.RaiseEventAsync(instanceId, SignalRConstants.ClientSendPaymentIntent, paymentSuccess);
    }
}