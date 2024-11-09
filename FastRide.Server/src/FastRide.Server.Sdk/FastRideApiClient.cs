using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
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

    public async Task<ApiResponseMessage<UserTypeResponse>> GetUserTypeAsync(string nameIdentifier, string email)
    {
        try
        {
            var task = _apiClient.GetUserTypeAsync(nameIdentifier, email);
            var result = await Execute(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error executing {nameof(GetUserTypeAsync)}");
            throw;
        }
    }
}