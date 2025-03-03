using Microsoft.AspNetCore.SignalR;

namespace FastRide.Server.Authentication;

public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var httpContext = connection.GetHttpContext();
        if (httpContext != null)
        {
            if (httpContext.Request.Query.TryGetValue("sub", out var userId))
            {
                return userId;
            }

            if (httpContext.Request.Headers.TryGetValue("sub", out var headerUserId))
            {
                return headerUserId;
            }
        }

        return connection.User?.FindFirst("sub")?.Value ?? "unknown";
    }
}