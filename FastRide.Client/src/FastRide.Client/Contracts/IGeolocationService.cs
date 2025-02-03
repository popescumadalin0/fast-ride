using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Contracts;

public interface IGeolocationService
{
    ValueTask<Geolocation> GetGeolocationAsync();
}