using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.HttpResponse;
using FastRide.Server.Services.Contracts;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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

    [Function(nameof(GetUserAsync))]
    public async Task<IActionResult> GetUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{nameIdentifier}/{email}")]
        HttpRequest req,
        string nameIdentifier,
        string email)
    {
        _logger.LogInformation($"{nameof(GetUserAsync)} HTTP trigger function processed a request.");

        //todo: see if this works and if the proxy is needed
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string>() { Environment.GetEnvironmentVariable("Google:ClientId") }
        };

        var payload =  await GoogleJsonWebSignature.ValidateAsync(
            req.Headers["Authorization"].ToString().Split("Bearer ")[1], settings);

        var response = await _userService.GetUserType(nameIdentifier, email);

        return ApiServiceResult.CreateApiResult(response);
    }
}