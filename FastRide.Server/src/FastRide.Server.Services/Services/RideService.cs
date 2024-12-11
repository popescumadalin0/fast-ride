using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Models;
using Microsoft.Extensions.Logging;
using RideStatus = FastRide.Server.Services.Enums.RideStatus;

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
                Id = x.RowKey,
                TimeStamp = x.Timestamp!.Value.DateTime,
                    DriverEmail = x.DriverEmail,
                Status = (Server.Contracts.RideStatus)x.Status,
                UserEmail = x.PartitionKey,
                DestinationLat = x.DestinationLat,
                DestinationLng = x.DestinationLng,
                StartPointLat = x.StartPointLat,
                StartPointLng = x.StartPointLng,
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
                StartPointLat = ride.StartPointLat,
                StartPointLng = ride.StartPointLng,
                DestinationLat = ride.DestinationLat,
                DestinationLng = ride.DestinationLng,
                PartitionKey = email,
                RowKey = Guid.NewGuid().ToString(),
                Timestamp= ride.TimeStamp,
                DriverEmail = ride.DriverEmail,
                Status = (RideStatus)ride.Status,
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