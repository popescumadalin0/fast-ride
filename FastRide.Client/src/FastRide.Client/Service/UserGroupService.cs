using System.Threading.Tasks;
using FastRide.Client.Contracts;
using Majorsoft.Blazor.Components.Common.JsInterop.Geo;
using IGeolocationService = FastRide.Client.Contracts.IGeolocationService;

namespace FastRide.Client.Service;

public class UserGroupService : IUserGroupService
{
    private readonly IGeolocationService _geolocationService;

    private readonly ILocationService _locationService;

    public UserGroupService(IGeolocationService geolocationService, ILocationService locationService)
    {
        _geolocationService = geolocationService;
        _locationService = locationService;
    }

    public async Task<string> GetCurrentUserGroupNameAsync()
    {
        var geolocation = await _geolocationService.GetGeolocationAsync();

        var locality =
            await _locationService.GetLocalityByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var country = await _locationService.GetCountryByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var county = await _locationService.GetCountyByLatLongAsync(geolocation.Latitude, geolocation.Longitude);

        var groupName = $"{country}-{county}-{locality}";

        return groupName;
    }
}