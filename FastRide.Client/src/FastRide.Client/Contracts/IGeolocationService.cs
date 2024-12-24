using System.Threading.Tasks;
using FastRide.Client.Models;

namespace FastRide.Client.Contracts;

public interface IGeolocationService
{
    /// <summary>
    /// Get geolocation based on device.
    /// </summary>
    /// <returns></returns>
    Task<Geolocation> GetLocationAsync();

    /// <summary>
    /// Get location name based on latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    Task<string> GetLocationByLatLongAsync(double latitude, double longitude);

    /// <summary>
    /// Get locality name based on latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    Task<string> GetLocalityByLatLongAsync(double latitude, double longitude);
}