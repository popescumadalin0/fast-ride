using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using Refit;

namespace FastRide.Server.Sdk.Contracts;

/// <summary />
public interface IFastRideApi
{
    [Get("/api/user")]
    Task<User> GetCurrentUserAsync();
    
    [Post("/api/user")]
    Task<User> GetUserAsync([Body] UserIdentifier userIdentifier);

    [Get("/api/rides")]
    Task<List<Ride>> GetRidesByUserAsync();
    
    [Get("/api/users")]
    Task<List<User>> GetUsersAsync();
    
    [Put("/api/user")]
    Task UpdateUserAsync([Body] UpdateUserPayload updatePayload);
}