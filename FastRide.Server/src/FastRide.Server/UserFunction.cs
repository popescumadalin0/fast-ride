using System.IO;
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

public class UserFunction
{
    private readonly IUserService _userService;

    private readonly ILogger<UserFunction> _logger;

    public UserFunction(IUserService userService, ILogger<UserFunction> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize]
    [Function(nameof(GetUserAsync))]
    public async Task<IActionResult> GetUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetUserAsync)} HTTP trigger function processed a request.");

        string requestBody;
        using (var streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        var request = JsonConvert.DeserializeObject<UserIdentifier>(requestBody);
        
        var response = await _userService.GetUserType(request);

        return ApiServiceResponse.ApiServiceResult(response);
    }
    
    [Authorize]
    [Function(nameof(UpdateUserRatingAsync))]
    public async Task<IActionResult> UpdateUserRatingAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "user")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(UpdateUserRatingAsync)} HTTP trigger function processed a request.");

        string requestBody;
        using (var streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        var request = JsonConvert.DeserializeObject<UserRating>(requestBody);
        
        var response = await _userService.UpdateUserRating(request);

        return ApiServiceResponse.ApiServiceResult(response);
    }
}