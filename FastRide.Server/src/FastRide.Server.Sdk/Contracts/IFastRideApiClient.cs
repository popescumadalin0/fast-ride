using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Sdk.Refit;

namespace FastRide.Server.Sdk.Contracts;

/// <summary />
public interface IFastRideApiClient
{
    Task<ApiResponseMessage<UserTypeResponse>> GetUserTypeAsync(string nameIdentifier, string email);
}