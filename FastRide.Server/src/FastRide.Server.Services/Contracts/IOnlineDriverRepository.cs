using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Contracts;

public interface IOnlineDriverRepository
{
    Task<List<OnlineDriversEntity>> GetOnlineDriversByGroupNameAsync(string groupName);
    
    Task<OnlineDriversEntity> GetOnlineDriversByUserIdAsync(string userId);

    Task<Response> AddOnlineDriverAsync(OnlineDriversEntity ride);
    Task<Response> DeleteOnlineDriverAsync(string groupName, string userId);
}