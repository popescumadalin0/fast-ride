using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
    public async Task<SignalRGroupAction> JoinUserToGroup(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientJoinUserToGroup, "userId",
            "groupName")]
        SignalRInvocationContext invocationContext,
        [DurableClient] DurableTaskClient client, string userId, string groupName)
    {
        var instances = client.GetAllInstancesAsync(new OrchestrationQuery()
        {
            Statuses =
            [
                OrchestrationRuntimeStatus.Pending,
                OrchestrationRuntimeStatus.Running,
                OrchestrationRuntimeStatus.Suspended
            ]
        });

        await foreach (var instance in instances)
        {
            var customStatus = JsonConvert.DeserializeObject<NewRideInput>(instance.SerializedCustomStatus!);
            if (customStatus.User.NameIdentifier == userId && customStatus.Status is InternRideStatus.NewRideAvailable
                    or InternRideStatus.GoingToUser
                    or InternRideStatus.GoingToDestination
                    or InternRideStatus.Finished
                    or InternRideStatus.Cancelled)
            {
                groupName = instance.InstanceId;
            }
            else if (customStatus.Driver?.NameIdentifier == userId && customStatus.Status is InternRideStatus.GoingToUser
                         or InternRideStatus.GoingToDestination
                         or InternRideStatus.Finished
                         or InternRideStatus.Cancelled
                         )
            {
                groupName = instance.InstanceId;
            }
        }

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