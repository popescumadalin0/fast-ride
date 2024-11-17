using System.Threading.Tasks;
using FastRide.Server.HttpResponse;
using FastRide.Server.Services.Contracts;
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

        var response = await _userService.GetUserType(nameIdentifier, email);

        return ApiServiceResult.CreateApiResult(response);
    }
}