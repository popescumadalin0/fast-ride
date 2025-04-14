using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Authentication;
using FastRide.Server.Contracts.Constants;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.HttpResponse;
using FastRide.Server.Models;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserType = FastRide.Server.Services.Enums.UserType;

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

        var rides = await MapRidesAsync(instances);

        rides = rides.Where(x =>
            x.User.NameIdentifier == req.HttpContext.User.Claims.Single(claim => claim.Type == "sub").Value).ToList();

        return ApiServiceResponse.ApiServiceResult(new ServiceResponse<List<Ride>>(rides));
    }

    [Function(nameof(NotifyUserStateAsync))]
    [SignalROutput(HubName = SignalRConstants.HubName)]
    public async Task<List<SignalRMessageAction>> NotifyUserStateAsync(
        [TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo,
        [DurableClient] DurableTaskClient client)
    {
        _logger.LogInformation($"{nameof(NotifyUserStateAsync)} timer trigger function started.");

        var instances = client.GetAllInstancesAsync(new OrchestrationQuery()
        {
            Statuses =
            [
                OrchestrationRuntimeStatus.Pending,
                OrchestrationRuntimeStatus.Running,
                OrchestrationRuntimeStatus.Suspended
            ]
        });
        var rides = await MapRidesAsync(instances);

        return rides.Select(ride => new SignalRMessageAction(SignalRConstants.ServerNotifyState)
            { Arguments = [ride], GroupName = ride.Id, }).ToList();
    }

    private static async Task<List<Ride>> MapRidesAsync(AsyncPageable<OrchestrationMetadata> orchestrations)
    {
        var rides = new List<Ride>();
        await foreach (var instance in orchestrations)
        {
            var customStatus = JsonConvert.DeserializeObject<NewRideInput>(instance.SerializedCustomStatus!);

            rides.Add(new Ride()
            {
                Cost = customStatus.Cost,
                Driver = customStatus.Driver,
                User = customStatus.User,
                Id = instance.InstanceId,
                Status = customStatus.Status,
                DestinationLat = customStatus.Destination.Latitude,
                DestinationLng = customStatus.Destination.Longitude,
                StartPointLat = customStatus.StartPoint.Latitude,
                StartPointLng = customStatus.StartPoint.Longitude,
                TimeStamp = instance.LastUpdatedAt.UtcDateTime,
            });
        }

        return rides;
    }
}