using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
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
    public async Task CreateRide(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientSendPriceCalculation, "instanceId",
            "isConfirmed")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string instanceId,
        bool isConfirmed)
    {
        await client.RaiseEventAsync(instanceId, SignalRConstants.ClientSendPriceCalculation, isConfirmed);
    }
}