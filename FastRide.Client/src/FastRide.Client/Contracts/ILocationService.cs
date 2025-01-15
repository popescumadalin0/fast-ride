using System.Threading.Tasks;

namespace FastRide.Client.Contracts;

public interface ILocationService
{
    /// <summary>
    /// Get location name based on latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    Task<string> GetAddressByLatLongAsync(double latitude, double longitude);

    /// <summary>
    /// Get locality name based on latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    Task<string> GetLocalityByLatLongAsync(double latitude, double longitude);

    /// <summary>
    /// Get country name based on latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    Task<string> GetCountryByLatLongAsync(double latitude, double longitude);

    /// <summary>
    /// Get county name based on latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    Task<string> GetCountyByLatLongAsync(double latitude, double longitude);
}