using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Services.Models;

namespace FastRide.Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<User>> GetUserAsync(UserIdentifier user, string name = default);
    
    ServiceResponse<List<User>> GetUsers();
    
    Task<ServiceResponse<User>> GetUserByUserIdAsync(string userId);
    
    Task<ServiceResponse> UpdateUserAsync(UpdateUserPayload updateUserPayload);

    Task<ServiceResponse> UpdateUserRatingAsync(string userId, int rating);
}