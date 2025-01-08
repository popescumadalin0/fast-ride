using System.Threading.Tasks;
using FastRide.Client.Contracts;

namespace FastRide.Client.Service;

public class UserGroupService : IUserGroupService
{
    private readonly IGeolocationService _geolocationService;

    public UserGroupService(IGeolocationService geolocationService)
    {
        _geolocationService = geolocationService;
    }

    public async Task<string> GetCurrentUserGroupNameAsync()
    {
        var geolocation = await _geolocationService.GetCoordonatesAsync();
        var locatlity =
            await _geolocationService.GetLocalityByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var country = await _geolocationService.GetCountryByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var county = await _geolocationService.GetCountyByLatLongAsync(geolocation.Latitude, geolocation.Longitude);

        var groupName = $"{country}-{county}-{locatlity}";

        return groupName;
    }
}