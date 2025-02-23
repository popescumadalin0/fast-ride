using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace FastRide.Client.Service;

public class LocationService : ILocationService
{
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;

    private readonly ILogger<LocationService> _logger;

    public LocationService(IJSRuntime jsRuntime, IConfiguration configuration, ILogger<LocationService> logger)
    {
        _jsRuntime = jsRuntime;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<string> GetAddressByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var formattedAddress = result.display_name;

        return formattedAddress;
    }

    public async Task<string> GetLocalityByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            result.Address.City;

        return locality;
    }

    public async Task<string> GetCountryByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            result.Address.Country;

        return locality;
    }

    public async Task<string> GetCountyByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            result.Address.County;

        return locality;
    }

    private async Task<OpenStreetMapResponse> GetInformationByLatLong(double latitude, double longitude)
    {
        var mapBaseUrl = _configuration.GetValue<string>("Map:BaseUrl");

        using var httpClient = new HttpClient();
        var response =
            await httpClient.GetAsync(
                new Uri($"{mapBaseUrl}/reverse?lat={latitude}&lon={longitude}&format=json"));
        var json = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject<OpenStreetMapResponse>(json);

        return result;
    }
}