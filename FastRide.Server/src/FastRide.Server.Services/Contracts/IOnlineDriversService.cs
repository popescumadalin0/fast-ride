using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Services.Models;

namespace FastRide.Server.Services.Contracts;

public interface IOnlineDriversService
{
    Task<ServiceResponse<List<OnlineDriver>>> GetClosestDriversByUserAsync(string groupName,
        Geolocation geolocation);

    Task<ServiceResponse<List<OnlineDriver>>> GetOnlineDriversByGroupName(string groupName);
    Task<ServiceResponse> AddOnlineDriverAsync(OnlineDriver onlineDriver);
}