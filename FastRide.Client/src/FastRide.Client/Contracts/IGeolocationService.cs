using System;
using System.Threading.Tasks;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Contracts;

public interface IGeolocationService
{
    ValueTask RequestGeoLocationAsync(bool enableHighAccuracy, int maximumAgeInMilliseconds);
    ValueTask RequestGeoLocationAsync();
    event Func<Geolocation, ValueTask> CoordinatesChanged;
    event Func<GeolocationError, ValueTask> OnGeolocationPositionError;
    Task OnSuccessAsync(Geolocation coordinates);
    Task OnErrorAsync(GeolocationError error);
}