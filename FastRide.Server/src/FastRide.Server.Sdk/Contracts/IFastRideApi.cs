using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using Refit;

namespace FastRide.Server.Sdk.Contracts;

/// <summary />
public interface IFastRideApi
{
    [Post("/api/user/{nameIdentifier}/{email}")]
    Task<UserTypeResponse> GetUserTypeAsync([Body] UserIdentifier user);

    [Get("/api/rides")]
    Task<List<Ride>> GetRidesByUserAsync();

    [Post("/api/ride")]
    Task AddRideAsync([Body] Ride ride);
    
    [Put("/api/user")]
    Task UpdateUserRatingAsync([Body] UserRating user);
}