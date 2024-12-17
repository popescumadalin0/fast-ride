using FastRide.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class SignalRUserFunction
{
    private readonly ILogger<SignalRUserFunction> _logger;

    public SignalRUserFunction(ILogger<SignalRUserFunction> logger)
    {
        _logger = logger;
    }

    [Function("JoinUserToGroup")]
    [SignalROutput(HubName = "serverless")]
    public SignalRGroupAction JoinUserToGroup(
        [SignalRTrigger("serverless", "messages", "JoinUserToGroup", "userId", "groupName")]
        SignalRInvocationContext invocationContext, string userId, string groupName)
    {
        return new SignalRGroupAction(SignalRGroupActionType.Add)
        {
            GroupName = groupName,
            UserId = userId
        };
    }

    [Function("LeaveUserFromGroup")]
    [SignalROutput(HubName = "serverless")]
    public SignalRGroupAction LeaveUserFromGroup(
        [SignalRTrigger("serverless", "messages", "LeaveUserFromGroup", "userId", "groupName")]
        SignalRInvocationContext invocationContext, string userId, string groupName)
    {
        return new SignalRGroupAction(SignalRGroupActionType.Remove)
        {
            GroupName = groupName,
            UserId = userId
        };
    }
}