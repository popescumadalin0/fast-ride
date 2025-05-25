using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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
    private readonly ILogger<LocationService> _logger;

    private readonly string _mapBaseUrl;

    public LocationService(IConfiguration configuration, ILogger<LocationService> logger)
    {
        _logger = logger;

        _mapBaseUrl = configuration.GetValue<string>("Map:BaseUrl");
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

    public async Task<List<OpenStreetMapResponse>> GetAddressesBySuggestions(string city, string query, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        var response =
            await httpClient.GetAsync(
                new Uri($"{_mapBaseUrl}/search?q={city} {query}&format=json&addressdetails=1"), cancellationToken: cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonConvert.DeserializeObject<List<OpenStreetMapResponse>>(json);

        return result;
    }

    private async Task<OpenStreetMapResponse> GetInformationByLatLong(double latitude, double longitude)
    {
        using var httpClient = new HttpClient();
        var response =
            await httpClient.GetAsync(
                new Uri($"{_mapBaseUrl}/reverse?lat={latitude}&lon={longitude}&format=json"));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<OpenStreetMapResponse>(json);

        return result;
    }
}