using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Services.Models;

namespace FastRide.Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<User>> GetUserAsync(UserIdentifier user);
    
    Task<ServiceResponse> UpdateUserAsync(UserIdentifier user, UpdateUserPayload updateUserPayload, string pictureUrl);
    
    Task<ServiceResponse> UpdateUserRatingAsync(UserRating userRating);
}