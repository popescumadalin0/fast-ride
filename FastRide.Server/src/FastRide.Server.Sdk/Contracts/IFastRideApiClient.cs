using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Sdk.Refit;

namespace FastRide.Server.Sdk.Contracts;

/// <summary />
public interface IFastRideApiClient
{
    Task<ApiResponseMessage<UserTypeResponse>> GetUserTypeAsync(UserIdentifier user);
    Task<ApiResponseMessage<List<Ride>>> GetRidesByUserAsync();
    
    Task<ApiResponseMessage> AddRideAsync(Ride ride);
    
    Task<ApiResponseMessage> UpdateUserRatingAsync(UserRating user);
}