using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastRide.Proxy.Controllers;

[Route("/api")]
[ApiController]
public class UserFunction(ILogger<UserFunction> logger, IFastRideApiClient fastRideApiClient)
    : ControllerBase
{
    //todo: map the response correctly
    [HttpGet("published-workflow-definition-codes")]
    public async Task<IActionResult> GetUserAsync(
        string nameIdentifier,
        string email)
    {
        logger.LogInformation($"{nameof(GetUserAsync)} HTTP trigger proxy processed a request.");

        var response = await fastRideApiClient.GetUserTypeAsync(nameIdentifier, email);

        return response;
    }
}