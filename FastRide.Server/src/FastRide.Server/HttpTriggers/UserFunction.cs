﻿using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Server.Authentication;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using FastRide.Server.HttpResponse;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FastRide.Server.HttpTriggers;

public class UserFunction
{
    private readonly IUserService _userService;

    private readonly ILogger<UserFunction> _logger;

    public UserFunction(IUserService userService, ILogger<UserFunction> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize(UserRoles = [UserType.User, UserType.Driver, UserType.Admin])]
    [Function(nameof(GetCurrentUserAsync))]
    public async Task<IActionResult> GetCurrentUserAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetCurrentUserAsync)} HTTP trigger function processed a request.");

        var response = await _userService.GetUserAsync(new UserIdentifier()
            {
                NameIdentifier = req.HttpContext.User.Claims.Single(x => x.Type == "sub").Value,
                Email = req.HttpContext.User.Claims.Single(x => x.Type == "email").Value
            },
            req.HttpContext.User.Claims.Single(x => x.Type == "name").Value);

        if (response.Success)
        {
            if (response.Response.PictureUrl != req.HttpContext.User.Claims.Single(x => x.Type == "picture").Value)
            {
                var update = await _userService.UpdateUserAsync(
                    new UpdateUserPayload()
                    {
                        User = response.Response.Identifier,
                        PhoneNumber = response.Response.PhoneNumber,
                        PictureUrl = req.HttpContext.User.Claims.Single(x => x.Type == "picture").Value
                    });

                if (!update.Success)
                {
                    return ApiServiceResponse.ApiServiceResult(update);
                }
            }
        }

        return ApiServiceResponse.ApiServiceResult(response);
    }

    [Authorize(UserRoles = [UserType.User, UserType.Driver, UserType.Admin])]
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

    [Authorize(UserRoles = [UserType.Admin])]
    [Function(nameof(GetUsers))]
    public IActionResult GetUsers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")]
        HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetUsers)} HTTP trigger function processed a request.");

        var response = _userService.GetUsers();

        return ApiServiceResponse.ApiServiceResult(response);
    }

    [Authorize(UserRoles = [UserType.User, UserType.Driver, UserType.Admin])]
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

        var user = await _userService.GetUserAsync(new UserIdentifier()
        {
            NameIdentifier = req.HttpContext.User.Claims.Single(x => x.Type == "sub").Value,
            Email =  req.HttpContext.User.Claims.Single(x => x.Type == "email").Value
        });
        var userType = user.Response.UserType.ToString();
        
        if (userType != UserType.Admin.ToString())
        {
            request.UserType = null;
        }

        var response = await _userService.UpdateUserAsync(
            request);

        return ApiServiceResponse.ApiServiceResult(response);
    }
}