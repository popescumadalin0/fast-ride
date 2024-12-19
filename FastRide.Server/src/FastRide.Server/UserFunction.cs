using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Authentication;
using FastRide.Server.Contracts.Models;
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
    [Function(nameof(GetCurrentUserAsync))]
    public async Task<IActionResult> GetCurrentUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetUserAsync)} HTTP trigger function processed a request.");

        var response = await _userService.GetUserAsync(new UserIdentifier()
        {
            NameIdentifier = req.HttpContext.User.Claims.Single(x => x.Type == "sub").Value,
            Email = req.HttpContext.User.Claims.Single(x => x.Type == "email").Value
        });

        if (response.Success)
        {
            if (response.Response.PictureUrl != req.HttpContext.User.Claims.Single(x => x.Type == "picture").Value)
            {
                var update = await _userService.UpdateUserAsync(new UserIdentifier()
                    {
                        NameIdentifier = response.Response.Identifier.NameIdentifier,
                        Email = response.Response.Identifier.Email,
                    },
                    new UpdateUserPayload()
                    {
                        PhoneNumber = response.Response.PhoneNumber,
                    },
                    req.HttpContext.User.Claims.Single(x => x.Type == "picture").Value);

                if (!update.Success)
                {
                    return ApiServiceResponse.ApiServiceResult(update);
                }
            }
        }

        return ApiServiceResponse.ApiServiceResult(response);
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

        var response = await _userService.GetUserAsync(request);

        return ApiServiceResponse.ApiServiceResult(response);
    }

    [Authorize]
    [Function(nameof(UpdateUserAsync))]
    public async Task<IActionResult> UpdateUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "user")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(UpdateUserAsync)} HTTP trigger function processed a request.");

        string requestBody;
        using (var streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        var request = JsonConvert.DeserializeObject<UpdateUserPayload>(requestBody);

        var response = await _userService.UpdateUserAsync(
            new UserIdentifier()
            {
                NameIdentifier = req.HttpContext.User.Claims.Single(x => x.Type == "sub").Value,
                Email = req.HttpContext.User.Claims.Single(x => x.Type == "email").Value
            },
            request,
            req.HttpContext.User.Claims.Single(x => x.Type == "picture").Value);

        return ApiServiceResponse.ApiServiceResult(response);
    }

    [Authorize]
    [Function(nameof(UpdateUserRatingAsync))]
    public async Task<IActionResult> UpdateUserRatingAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "user/rating")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(UpdateUserRatingAsync)} HTTP trigger function processed a request.");

        string requestBody;
        using (var streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        var request = JsonConvert.DeserializeObject<UserRating>(requestBody);

        var response = await _userService.UpdateUserRatingAsync(request);

        return ApiServiceResponse.ApiServiceResult(response);
    }
}