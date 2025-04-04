using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Authentication;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.HttpResponse;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Enums;
using FastRide.Server.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FastRide.Server.HttpTriggers;

public class RideFunction
{
    private readonly ILogger<RideFunction> _logger;

    public RideFunction(ILogger<RideFunction> logger)
    {
        _logger = logger;
    }

    [Authorize(UserRoles = [UserType.User, UserType.Driver])]
    [Function(nameof(GetRidesByUserAsync))]
    public async Task<IActionResult> GetRidesByUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rides")]
        HttpRequest req,
        [DurableClient] DurableTaskClient client)
    {
        _logger.LogInformation($"{nameof(GetRidesByUserAsync)} HTTP trigger function processed a request.");

        var instances = client.GetAllInstancesAsync();

        var rides = new List<Ride>();
        await foreach (var instance in instances)
        {
            var customStatus = JsonConvert.DeserializeObject<NewRideInput>(instance.SerializedCustomStatus!);
            if (customStatus.User.NameIdentifier == req.HttpContext.User.Claims.Single(x => x.Type == "sub").Value)
            {
                rides.Add(new Ride()
                {
                    Cost = customStatus.Cost,
                    Driver = customStatus.Driver,
                    User = customStatus.User,
                    Id = instance.InstanceId,
                    Status = instance.RuntimeStatus.ToRideStatus(),
                    DestinationLat = customStatus.Destination.Latitude,
                    DestinationLng = customStatus.Destination.Longitude,
                    StartPointLat = customStatus.StartPoint.Latitude,
                    StartPointLng = customStatus.StartPoint.Longitude,
                    TimeStamp = instance.LastUpdatedAt.UtcDateTime,
                });
            }
        }

        return ApiServiceResponse.ApiServiceResult(new ServiceResponse<List<Ride>>(rides));
    }
}