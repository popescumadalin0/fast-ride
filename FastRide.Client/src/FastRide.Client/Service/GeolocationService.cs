using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class GeolocationService : IGeolocationService
{
    private readonly IJSRuntime _jsRuntime;

    public GeolocationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<Geolocation> GetLocationAsync()
    {
        return await _jsRuntime.InvokeAsync<Geolocation>("window.getGeolocation");
    }
}