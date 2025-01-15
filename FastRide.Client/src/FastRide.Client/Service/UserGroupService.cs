using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Models;

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
        var tcs = new TaskCompletionSource<Geolocation>();

        Func<Geolocation, ValueTask> coordinatesChangedHandler = null;
        coordinatesChangedHandler = (geolocation) =>
        {
            tcs.SetResult(geolocation);

            _geolocationService.CoordinatesChanged -= coordinatesChangedHandler;

            return ValueTask.CompletedTask;
        };

        _geolocationService.CoordinatesChanged += coordinatesChangedHandler;

        await _geolocationService.RequestGeoLocationAsync();

        var geolocation = await tcs.Task;

        var locality =
            await _locationService.GetLocalityByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var country = await _locationService.GetCountryByLatLongAsync(geolocation.Latitude, geolocation.Longitude);
        var county = await _locationService.GetCountyByLatLongAsync(geolocation.Latitude, geolocation.Longitude);

        var groupName = $"{country}-{county}-{locality}";

        return groupName;
    }
}