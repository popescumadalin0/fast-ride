using System.Threading;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Enums;
using FastRide.Server.Services.Models;

namespace FastRide.Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<UserTypeResponse>> GetUserType(UserIdentifier user);
    
    Task<ServiceResponse> UpdateUserRating(UserRating userRating);
}