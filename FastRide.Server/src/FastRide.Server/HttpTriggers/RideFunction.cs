using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Authentication;
using FastRide.Server.HttpResponse;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FastRide.Server.HttpTriggers;

public class RideFunction
{
    private readonly IRideService _rideService;

    private readonly ILogger<RideFunction> _logger;

    public RideFunction(IRideService rideService, ILogger<RideFunction> logger)
    {
        _rideService = rideService;
        _logger = logger;
    }

    [Authorize(UserRoles = [UserType.User, UserType.Driver])]
    [Function(nameof(GetRidesByUserAsync))]
    public async Task<IActionResult> GetRidesByUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rides")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetRidesByUserAsync)} HTTP trigger function processed a request.");

        var response =
            await _rideService.GetRidesByUser(req.HttpContext.User.Claims.Single(x => x.Type == "email").Value);

        return ApiServiceResponse.ApiServiceResult(response);
    }
}