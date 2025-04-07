using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class DriverArrivedTrigger
{
    private readonly ILogger<DriverArrivedTrigger> _logger;

    public DriverArrivedTrigger(ILogger<DriverArrivedTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(DriverArrivedTrigger))]
    public async Task DriverArrived(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientDriverArrived, "groupName")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client,
        string groupName)
    {
        _logger.LogInformation($"{nameof(SendPriceCalculationTrigger)} function executed");

        await client.RaiseEventAsync(groupName, SignalRConstants.ClientDriverArrived);
    }
}