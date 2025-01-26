using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class SendPriceCalculationTrigger
{
    private readonly ILogger<SendPriceCalculationTrigger> _logger;

    public SendPriceCalculationTrigger(ILogger<SendPriceCalculationTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SendPriceCalculationTrigger))]
    public async Task SendPriceCalculation(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientSendPriceCalculation, "instanceId",
            "priceConfirmed")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string instanceId,
        decimal priceConfirmed)
    {
        _logger.LogInformation($"{nameof(SendPriceCalculationTrigger)} function executed");

        await client.RaiseEventAsync(instanceId, SignalRConstants.ClientSendPriceCalculation, priceConfirmed);
    }
}