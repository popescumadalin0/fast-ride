﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Enums;

namespace FastRide.Server.Services.Repositories;

public class RideRepository : IRideRepository
{
    private readonly ITableClient<RideEntity> _rideTable;

    public RideRepository(ITableClient<RideEntity> rideTable)
    {
        this._rideTable = rideTable;
    }

    public Task<List<RideEntity>> GetRidesByUser(string email)
    {
        
        var rides = _rideTable.GetBy(x=> x.PartitionKey == email);
        
        if (rides is not { Count: 0 })
        {
            return null;
        }

        return Task.FromResult(rides);
    }

    public async Task<Response> AddRideForUser(RideEntity ride)
    {
        var response = await _rideTable.AddOrUpdateAsync(ride);

        return response;
    }
}