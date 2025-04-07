using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using IGeolocationService = FastRide.Client.Contracts.IGeolocationService;

namespace FastRide.Client.Service;

public class UserGroupService : IUserGroupService
{
    private readonly IGeolocationService _geolocationService;

    private readonly ILocationService _locationService;

    private readonly ICurrentRideState _currentRideState;

    public UserGroupService(IGeolocationService geolocationService, ILocationService locationService,
        ICurrentRideState currentRideState)
    {
        _geolocationService = geolocationService;
        _locationService = locationService;
        _currentRideState = currentRideState;
    }

    public async Task<string> GetCurrentUserGroupNameAsync()
    {
        if (_currentRideState.State != RideStatus.None &&
            _currentRideState.State != RideStatus.Cancelled &&
            _currentRideState.State != RideStatus.Finished)
        {
            return _currentRideState.InstanceId;
        }

        var geolocation = await _geolocationService.GetGeolocationAsync();

        var locality =
            await _locationService.GetLocalityByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var country = await _locationService.GetCountryByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var county = await _locationService.GetCountyByLatLongAsync(geolocation.Latitude, geolocation.Longitude);

        var groupName = $"{country}-{county}-{locality}";

        return groupName;
    }
}