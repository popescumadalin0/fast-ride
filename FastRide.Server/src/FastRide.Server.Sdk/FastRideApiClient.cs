using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using FastRide.Server.Sdk.Refit;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.Sdk;

/// <summary>
/// Here we add the endpoints for the entire application
/// </summary>
public class FastRideApiClient : RefitApiClient<IFastRideApi>, IFastRideApiClient
{
    private readonly IFastRideApi _apiClient;

    private readonly ILogger<FastRideApiClient> _logger;

    public FastRideApiClient(IFastRideApi apiClient, ILogger<FastRideApiClient> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<ApiResponseMessage<User>> GetCurrentUserAsync()
    {
        try
        {
            var task = _apiClient.GetCurrentUserAsync();
            var result = await Execute(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(GetCurrentUserAsync)}");
            throw;
        }
    }
    
    public async Task<ApiResponseMessage<User>> GetUserAsync(UserIdentifier userIdentifier)
    {
        try
        {
            var task = _apiClient.GetUserAsync(userIdentifier);
            var result = await Execute(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(GetUserAsync)}");
            throw;
        }
    }
    
    public async Task<ApiResponseMessage<List<Ride>>> GetRidesByUserAsync()
    {
        try
        {
            var task = _apiClient.GetRidesByUserAsync();
            var result = await Execute(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(GetRidesByUserAsync)}");
            throw;
        }
    }

    /*
    public async Task<ApiResponseMessage> AddRideAsync(Ride ride)
    {
        try
        {
            var task = _apiClient.AddRideAsync(ride);
            var result = await ExecuteWithNoContentResponse(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(AddRideAsync)}");
            throw;
        }
    }

    public async Task<ApiResponseMessage> UpdateUserRatingAsync(UserRating user)
    {
        try
        {
            var task = _apiClient.UpdateUserRatingAsync(user);
            var result = await ExecuteWithNoContentResponse(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(UpdateUserRatingAsync)}");
            throw;
        }
    }
    */

    public async Task<ApiResponseMessage> UpdateUserAsync(UpdateUserPayload updateUserPayload)
    {
        try
        {
            var task = _apiClient.UpdateUserAsync(updateUserPayload);
            var result = await ExecuteWithNoContentResponse(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(UpdateUserAsync)}");
            throw;
        }
    }
}