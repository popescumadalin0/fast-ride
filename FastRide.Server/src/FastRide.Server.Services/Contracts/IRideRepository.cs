using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Contracts;

public interface IRideRepository
{
    Task<List<RideEntity>> GetRidesByUser(string email);
    Task<Response> AddRideForUser(RideEntity ride);
}