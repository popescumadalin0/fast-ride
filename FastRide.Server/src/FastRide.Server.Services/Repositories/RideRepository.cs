using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Repositories;

public class RideRepository : IRideRepository
{
    private readonly ITableClient<RideEntity> _rideTable;

    public RideRepository(ITableClient<RideEntity> rideTable)
    {
        this._rideTable = rideTable;
    }

    public Task<List<RideEntity>> GetRidesByUserAsync(string email)
    {
        var rides = _rideTable.GetBy(x => x.PartitionKey == email);

        return Task.FromResult(rides);
    }

    public async Task<Response> AddRideForUserAsync(RideEntity ride)
    {
        var response = await _rideTable.AddOrUpdateAsync(ride);

        return response;
    }
}