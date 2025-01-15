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

        var formattedAddress = result.results[0].formatted_address;

        return formattedAddress;
    }

    public async Task<string> GetLocalityByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            result.results[0].address_components
                .Single(x => x.types.Contains("locality")).long_name;

        return locality;
    }

    public async Task<string> GetCountryByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            result.results[0].address_components
                .Single(x => x.types.Contains("country")).long_name;

        return locality;
    }

    public async Task<string> GetCountyByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            result.results[0].address_components
                .Single(x => x.types.Contains("administrative_area_level_1")).long_name;

        return locality;
    }

    private async Task<GoogleLocationResponse> GetInformationByLatLong(double latitude, double longitude)
    {
        var googleMapsBaseUrl = _configuration.GetValue<string>("GoogleMaps:BaseUrl");
        var googleMapsApiKey = _configuration.GetValue<string>("GoogleMaps:ApiKey");

        using var httpClient = new HttpClient();
        var response =
            await httpClient.GetAsync(
                new Uri($"{googleMapsBaseUrl}/geocode/json?latlng={latitude},{longitude}&key={googleMapsApiKey}"));
        var json = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject<GoogleLocationResponse>(json);

        return result;
    }
}