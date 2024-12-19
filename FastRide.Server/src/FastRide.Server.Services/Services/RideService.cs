using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
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
                Driver = new UserIdentifier()
                {
                    Email = x.DriverEmail,
                    NameIdentifier = x.DriverId
                },
                Status = (Server.Contracts.Enums.RideStatus)x.Status,
                User = new UserIdentifier()
                {
                    Email = x.PartitionKey,
                    NameIdentifier = x.UserId
                },
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

    public async Task<ServiceResponse> AddRideAsync(Ride ride)
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
                PartitionKey = ride.User.Email,
                RowKey = Guid.NewGuid().ToString(),
                Timestamp = ride.TimeStamp,
                DriverEmail = ride.Driver.Email,
                DriverId = ride.Driver.NameIdentifier,
                UserId = ride.User.NameIdentifier,
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