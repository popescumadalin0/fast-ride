using System;
using System.Collections.Generic;
using System.Dynamic;
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

public class GeolocationService : IGeolocationService
{
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;

    private readonly ILogger<GeolocationService> _logger;

    public GeolocationService(IJSRuntime jsRuntime, IConfiguration configuration, ILogger<GeolocationService> logger)
    {
        _jsRuntime = jsRuntime;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Geolocation> GetCoordonatesAsync()
    {
        return await _jsRuntime.InvokeAsync<Geolocation>("window.getGeolocation");
    }

    public async Task<string> GetAddressByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var formattedAddress = result.results[0].formatted_address.ToString();

        return formattedAddress;
    }

    public async Task<string> GetLocalityByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            ((List<dynamic>)result.results[0].address_components.ToList())
            .Single(x => ((List<string>)x.types).Contains("locality")).ToString();

        return locality;
    }

    public async Task<string> GetCountryByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            ((List<dynamic>)result.results[0].address_components.ToList())
            .Single(x => ((List<string>)x.types).Contains("country")).ToString();

        return locality;
    }

    public async Task<string> GetCountyByLatLongAsync(double latitude, double longitude)
    {
        var result = await GetInformationByLatLong(latitude, longitude);

        var locality =
            ((List<dynamic>)result.results[0].address_components.ToList())
            .Single(x => ((List<string>)x.types).Contains("administrative_area_level_1")).ToString();

        return locality;
    }

    private async Task<dynamic> GetInformationByLatLong(double latitude, double longitude)
    {
        var googleMapsBaseUrl = _configuration.GetValue<string>("GoogleMaps:BaseUrl");
        var googleMapsApiKey = _configuration.GetValue<string>("GoogleMaps:ApiKey");

        using var httpClient = new HttpClient();
        var response =
            await httpClient.GetAsync(
                new Uri($"{googleMapsBaseUrl}/geocode/json?latlng={latitude},{longitude}&key={googleMapsApiKey}"));
        var json = await response.Content.ReadAsStringAsync();

        dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(json);

        return result;
    }
}