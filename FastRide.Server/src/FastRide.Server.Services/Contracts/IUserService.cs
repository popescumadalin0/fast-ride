using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.Services.Models;

namespace FastRide.Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<User>> GetUserAsync(UserIdentifier user);
    
    Task<ServiceResponse<User>> GetUserByUserIdAsync(string userId);
    
    Task<ServiceResponse> UpdateUserAsync(UserIdentifier user, UpdateUserPayload updateUserPayload, string pictureUrl);
    
    Task<ServiceResponse> UpdateUserRatingAsync(UserRating userRating);
}