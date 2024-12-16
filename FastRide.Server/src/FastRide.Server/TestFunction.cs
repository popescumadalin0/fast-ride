using Microsoft.Azure.Functions.Worker;

namespace FastRide.Server;

public class TestFunction
{
    //Send the messages!
    [Function("messages")]
    [SignalROutput(HubName = "serverless")]
    public static SignalRMessageAction  SendMessage(
        [SignalRTrigger("serverless", "messages", "SendMessage")]
        SignalRInvocationContext invocationContext,
        string message)
    {
        return new SignalRMessageAction("clientMessage")
        {
            // broadcast to all the connected clients without specifying any connection, user or group.
            Arguments = invocationContext.Arguments,
        };
    }
}

public class ClientMessage
{
    public string Name { get; set; }
    public string Message { get; set; }
}

/*
[SignalRConnection]
public class Functions : ServerlessHub
{
    private const string HubName = nameof(Functions); // Used by SignalR trigger only

    public Functions(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
    
   

    [Function("negotiate")]
    public async Task<HttpResponseData> Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var negotiateResponse = await NegotiateAsync(new() { UserId = req.Headers.GetValues("userId").FirstOrDefault() });
        var response = req.CreateResponse();
        response.WriteBytes(negotiateResponse.ToArray());
        return response;
    }

    [Function("Broadcast")]
    public Task Broadcast(
        [SignalRTrigger(HubName, "messages", "broadcast", "message")] SignalRInvocationContext invocationContext, string message)
    {
        return Clients.All.SendAsync("newMessage",  message);
    }

    [Function("JoinGroup")]
    public Task JoinGroup([SignalRTrigger(HubName, "messages", "JoinGroup", "connectionId", "groupName")] SignalRInvocationContext invocationContext, string connectionId, string groupName)
    {
        return Groups.AddToGroupAsync(connectionId, groupName);
    }
}*/