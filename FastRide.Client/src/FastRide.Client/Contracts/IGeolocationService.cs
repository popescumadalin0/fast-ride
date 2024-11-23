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
}