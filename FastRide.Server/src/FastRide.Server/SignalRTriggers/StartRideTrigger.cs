using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class StartRideTrigger
{
    private readonly ILogger<StartRideTrigger> _logger;

    public StartRideTrigger(ILogger<StartRideTrigger> logger)
    {
        _logger = logger;
    }

    /*[Function(nameof(StartRideTrigger))]
    [SignalROutput(HubName = "ride",  ConnectionStringSetting = "SignalRConnection")]
    public Task HttpStart(
        [SignalRTrigger("ride", "command", "start-ride", ConnectionStringSetting = "SignalRConnection")]
        SignalRInvocationContext invocationContext, FunctionContext functionContext)
    {
        // Function input comes from the request content.
        /*string instanceId =
            await starter.ScheduleNewOrchestrationInstanceAsync(nameof(StartNewRideOrchestration), null);
        _logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
        return return new MyMessage()
        {
            Target = "newMessage",
            Arguments = new[] { message }
        };
        return await starter.CreateCheckStatusResponseAsync(req, instanceId);#1#

        return Task.CompletedTask;
    }*/
    
    [Function("negotiate")]
    public static HttpResponseData Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req,
        [SignalRConnectionInfoInput] string connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(connectionInfo);
        return response;
    }
}