using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Models;
using Microsoft.Extensions.Logging;
using UserType = FastRide.Server.Services.Enums.UserType;

namespace FastRide.Server.Services.Services;

public class RideService : IRideService
{
    private readonly IRideRepository _rideRepository;
    private readonly ILogger<RideService> _logger;

    public RideService(IRideRepository rideRepository, ILogger<RideService> logger)
    {
        _rideRepository = rideRepository;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Ride>>> GetRidesByUser(string email)
    {
        try
        {
            var rides = await _rideRepository.GetRidesByUser(email);

            return new ServiceResponse<List<Ride>>(rides.Select(x => new Ride
            {
                Cost = x.Cost,
                Destination = x.Destination,
                Id = x.RowKey,
                FinishTime = x.FinishTime,
                DriverEmail = x.DriverEmail,
            }).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<List<Ride>>(ex);
        }
    }

    public async Task<ServiceResponse> AddRideAsync(Ride ride, string email)
    {
        try
        {
            await _rideRepository.AddRideForUser(new RideEntity()
            {
                Cost = ride.Cost,
                Destination = ride.Destination,
                PartitionKey = email,
                RowKey = Guid.NewGuid().ToString(),
                FinishTime = ride.FinishTime,
                DriverEmail = ride.DriverEmail,
            });

            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse(ex);
        }
    }
}