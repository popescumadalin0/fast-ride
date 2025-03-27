using System.Threading.Tasks;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.SignalRTriggers;

public class NotifyUserGeolocationTrigger
{
    private readonly ILogger<NotifyUserGeolocationTrigger> _logger;
    
    private IOnlineDriversService  _onlineDriversService;
    
    private IUserService  _userService;

    public NotifyUserGeolocationTrigger(ILogger<NotifyUserGeolocationTrigger> logger, IOnlineDriversService onlineDriversService, IUserService userService)
    {
        _logger = logger;
        _onlineDriversService = onlineDriversService;
        _userService = userService;
    }

    [Function(nameof(NotifyUserGeolocationTrigger))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<SignalRMessageAction> NotifyUserGeolocation(
        [SignalRTrigger(SignalRConstants.HubName, "messages", SignalRConstants.ClientNotifyUserGeolocation, "userId",
            "groupName", "geolocation")]
        SignalRInvocationContext invocationContext, string userId, string groupName, Geolocation geolocation)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);

        if (user.Response.UserType == UserType.Driver)
        {
            await _onlineDriversService.AddOnlineDriverAsync(new OnlineDriver()
            {
                Identifier = user.Response.Identifier,
                Geolocation =  geolocation,
                GroupName =  groupName,
            });
        }

        return new SignalRMessageAction(SignalRConstants.ServerNotifyUserGeolocation)
        {
            Arguments =
            [
                new NotifyUserGeolocation()
                {
                    UserId = userId,
                    Geolocation = geolocation
                }
            ],
            GroupName = groupName,
        };
    }
}