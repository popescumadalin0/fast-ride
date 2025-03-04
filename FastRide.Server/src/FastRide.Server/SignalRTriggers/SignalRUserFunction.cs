using FastRide.Server.Contracts.Constants;
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

    [Function(nameof(JoinUserToGroup))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRGroupAction JoinUserToGroup(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientJoinUserToGroup, "userId",
            "groupName")]
        SignalRInvocationContext invocationContext, string userId, string groupName)
    {
        return new SignalRGroupAction(SignalRGroupActionType.Add)
        {
            GroupName = groupName,
            UserId = userId
        };
    }

    [Function(nameof(LeaveUserFromGroup))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public SignalRGroupAction LeaveUserFromGroup(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientLeaveUserFromGroup, "userId",
            "groupName")]
        SignalRInvocationContext invocationContext, string userId, string groupName)
    {
        return new SignalRGroupAction(SignalRGroupActionType.Remove)
        {
            GroupName = groupName,
            UserId = userId
        };
    }
}