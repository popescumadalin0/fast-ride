using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Authentication;
using FastRide.Server.Contracts;
using FastRide.Server.HttpResponse;
using FastRide.Server.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FastRide.Server;

public class RideFunction
{
    private readonly IRideService _rideService;

    private readonly ILogger<RideFunction> _logger;

    public RideFunction(IRideService rideService, ILogger<RideFunction> logger)
    {
        _rideService = rideService;
        _logger = logger;
    }

    [Authorize]
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
    
    [Authorize]
    [Function(nameof(AddRideAsync))]
    public async Task<IActionResult> AddRideAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ride")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(AddRideAsync)} HTTP trigger function processed a request.");
        
        string requestBody;
        using (var streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        var request = JsonConvert.DeserializeObject<Ride>(requestBody);
        
        var response =
            await _rideService.AddRideAsync(request, req.HttpContext.User.Claims.Single(x => x.Type == "email").Value);

        return ApiServiceResponse.ApiServiceResult(response);
    }
}