using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Models;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Services.Services;

public class OnlineDriversService : IOnlineDriversService
{
    private readonly IOnlineDriverRepository _onlineDriverRepository;
    private readonly ILogger<OnlineDriversService> _logger;

    private readonly IDistanceService _distanceService;

    public OnlineDriversService(IOnlineDriverRepository onlineDriverRepository, ILogger<OnlineDriversService> logger,
        IDistanceService distanceService)
    {
        _onlineDriverRepository = onlineDriverRepository;
        _logger = logger;
        _distanceService = distanceService;
    }

    public async Task<ServiceResponse<List<OnlineDriver>>> GetClosestDriversByUserAsync(string groupName,
        Geolocation geolocation)
    {
        try
        {
            var onlineDrivers = await _onlineDriverRepository.GetOnlineDriversByGroupNameAsync(groupName);

            var nearestDrivers = onlineDrivers
                .Select(driver => new
                {
                    Driver = driver,
                    Distance = _distanceService.GetDistanceBetweenLocations(geolocation,
                        new Geolocation() { Latitude = driver.Latitude, Longitude = driver.Longitude })
                })
                .OrderBy(d => d.Distance)
                .Select(x => x.Driver).ToList();

            return new ServiceResponse<List<OnlineDriver>>(nearestDrivers.Select(nearestDriver => new OnlineDriver
            {
                GroupName = nearestDriver.PartitionKey,
                Identifier = new UserIdentifier()
                {
                    Email = nearestDriver.Email,
                    NameIdentifier = nearestDriver.RowKey
                },
                Geolocation = new Geolocation
                {
                    Latitude = nearestDriver.Latitude,
                    Longitude = nearestDriver.Longitude
                }
            }).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<List<OnlineDriver>>(ex);
        }
    }

    public async Task<ServiceResponse<List<OnlineDriver>>> GetOnlineDriversByGroupName(string groupName)
    {
        try
        {
            var onlineDrivers = await _onlineDriverRepository.GetOnlineDriversByGroupNameAsync(groupName);

            return new ServiceResponse<List<OnlineDriver>>(onlineDrivers.Select(x => new OnlineDriver
            {
                GroupName = x.PartitionKey,
                Identifier = new UserIdentifier()
                {
                    Email = x.Email,
                    NameIdentifier = x.RowKey
                },
                Geolocation = new Geolocation
                {
                    Latitude = x.Latitude,
                    Longitude = x.Longitude
                }
            }).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<List<OnlineDriver>>(ex);
        }
    }

    public async Task<ServiceResponse> AddOnlineDriverAsync(OnlineDriver onlineDriver)
    {
        try
        {
            await _onlineDriverRepository.AddOnlineDriverAsync(new OnlineDriversEntity()
            {
                PartitionKey = onlineDriver.GroupName,
                RowKey = onlineDriver.Identifier.NameIdentifier,
                Timestamp = DateTime.UtcNow,
                Email = onlineDriver.Identifier.Email,
                Latitude = onlineDriver.Geolocation.Latitude,
                Longitude = onlineDriver.Geolocation.Longitude,
            });

            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse(ex);
        }
    }

    public async Task<ServiceResponse> DeleteOnlineDriverAsync(string userId)
    {
        try
        {
            var user = await _onlineDriverRepository.GetOnlineDriversByUserIdAsync(userId);

            if (user != null)
            {
                await _onlineDriverRepository.DeleteOnlineDriverAsync(user.PartitionKey, user.RowKey);
            }

            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse(ex);
        }
    }
}