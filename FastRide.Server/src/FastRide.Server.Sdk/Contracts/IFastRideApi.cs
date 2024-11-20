using System.Threading.Tasks;
using FastRide.Server.Contracts;
using Refit;

namespace FastRide.Server.Sdk.Contracts;

/// <summary />
public interface IFastRideApi
{
    [Get("/api/user/{nameIdentifier}/{email}")]
    Task<UserTypeResponse> GetUserTypeAsync(string nameIdentifier, string email);
}