﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Refit;

namespace FastRide.Server.Sdk.Contracts;

/// <summary />
public interface IFastRideApiClient
{
    Task<ApiResponseMessage<User>> GetCurrentUserAsync();

    Task<ApiResponseMessage<User>> GetUserAsync(UserIdentifier userIdentifier);
    
    Task<ApiResponseMessage<List<Ride>>> GetRidesByUserAsync();
    
    Task<ApiResponseMessage> UpdateUserAsync(UpdateUserPayload updateUserPayload);
    
    Task<ApiResponseMessage<List<User>>> GetUsersAsync();
}