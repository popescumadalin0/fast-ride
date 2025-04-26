using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json.Linq;

namespace FastRide.Client.Service;

public class GeolocationService : IGeolocationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<GeolocationService> _dotNetObjectReference;
    private readonly HttpClient _http;

    private AuthenticationStateProvider _authenticationStateProviderForMock;

    private event Func<Geolocation, ValueTask> CoordinatesChanged = default!;

    private event Func<GeolocationError, ValueTask> OnGeolocationPositionError = default!;
    
    private Dictionary<string, List<Geolocation>> _mocks = new();
    private Dictionary<string, int> _mockIndexes = new();

    public GeolocationService(IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProviderForMock, HttpClient http)
    {
        _jsRuntime = jsRuntime;
        _authenticationStateProviderForMock = authenticationStateProviderForMock;
        _http = http;

        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    public async ValueTask<Geolocation> GetGeolocationAsync()
    {
        var tcs = new TaskCompletionSource<Geolocation>();

        Func<Geolocation, ValueTask> coordinatesChangedHandler = null;
        coordinatesChangedHandler = (geolocation) =>
        {
            tcs.SetResult(geolocation);

            CoordinatesChanged -= coordinatesChangedHandler;

            return ValueTask.CompletedTask;
        };

        CoordinatesChanged += coordinatesChangedHandler;

        await RequestGeoLocationAsync();

        var geolocation = await tcs.Task;

        return geolocation;
    }

    [JSInvokable]
    public async Task OnSuccessAsync(Geolocation coordinates)
    {
        if (CoordinatesChanged != null!)
        {
            await CoordinatesChanged.Invoke(coordinates);
        }
    }

    [JSInvokable]
    public async Task OnErrorAsync(GeolocationError error)
    {
        if (OnGeolocationPositionError != null!)
        {
            await OnGeolocationPositionError.Invoke(error);
        }
    }
    
    private async Task InitializeMocksAsync()
    {
        try
        {
            var json = await _http.GetFromJsonAsync<Dictionary<string, List<Geolocation>>>("location-mock/mocks.json");
            _mocks = json ?? new Dictionary<string, List<Geolocation>>();
            _mockIndexes = _mocks.Keys.ToDictionary(key => key, _ => 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load mock data: {ex.Message}");
            _mocks = new Dictionary<string, List<Geolocation>>();
            _mockIndexes = new Dictionary<string, int>();
        }
    }

    private async ValueTask RequestGeoLocationAsync(bool enableHighAccuracy, int maximumAgeInMilliseconds)
    {
        var authState = await _authenticationStateProviderForMock.GetAuthenticationStateAsync();
        if (_mocks.Count == 0)
        {
            await InitializeMocksAsync();
        }
    
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            if (_mocks.TryGetValue(authState.User.Claims.First(x => x.Type == "sub").Value, out var geolocationList))
            {
                if (_mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value] >= geolocationList.Count)
                {
                    _mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value] = 0;
                }
    
                await OnSuccessAsync(geolocationList[_mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value]++]);
                return;
            }
        }
    
        await _jsRuntime.InvokeVoidAsync("window.getGeolocation",
            _dotNetObjectReference,
            enableHighAccuracy,
            maximumAgeInMilliseconds);
    }

    private async ValueTask RequestGeoLocationAsync()
    {
        await RequestGeoLocationAsync(enableHighAccuracy: true, maximumAgeInMilliseconds: 0);
    }
}