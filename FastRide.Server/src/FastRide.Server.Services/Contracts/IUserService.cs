using System.Threading;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Enums;
using FastRide.Server.Services.Models;
using UserType = FastRide.Server.Contracts.UserType;

namespace FastRide.Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<User>> GetUserAsync(UserIdentifier user);
    
    Task<ServiceResponse> UpdateUserAsync(UserIdentifier user, UpdateUserPayload updateUserPayload);
    
    Task<ServiceResponse> UpdateUserRatingAsync(UserRating userRating);
}