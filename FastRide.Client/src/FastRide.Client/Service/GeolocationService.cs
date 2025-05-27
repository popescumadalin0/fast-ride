using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class GeolocationService : IGeolocationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ICurrentRideState _currentRideState;
    private readonly DotNetObjectReference<GeolocationService> _dotNetObjectReference;
    private readonly HttpClient _http;

    private AuthenticationStateProvider _authenticationStateProviderForMock;

    private event Func<Geolocation, ValueTask> CoordinatesChanged = default!;

    private event Func<GeolocationError, ValueTask> OnGeolocationPositionError = default!;

    private Dictionary<string, List<Geolocation>> _mocks = new();
    private Dictionary<string, int> _mockIndexes = new();

    private int _stay = 0;

    public GeolocationService(IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProviderForMock,
        HttpClient http, ICurrentRideState currentRideState)
    {
        _jsRuntime = jsRuntime;
        _authenticationStateProviderForMock = authenticationStateProviderForMock;
        _http = http;
        _currentRideState = currentRideState;

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
        #if DEBUG
        if (await HandleGeolocationMockAsync())
        {
            return;
        }
        #endif

        await _jsRuntime.InvokeVoidAsync("window.getGeolocation",
            _dotNetObjectReference,
            enableHighAccuracy,
            maximumAgeInMilliseconds);
    }

    private async Task<bool> HandleGeolocationMockAsync()
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
                //reset position
                if (_mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value] >= geolocationList.Count)
                {
                    _mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value] = 0;
                }

                var nextGeolocation =
                    geolocationList[_mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value]];
                var userRole = authState.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

                //move user if needed
                if (userRole == UserType.User.ToString() &&
                    _currentRideState.State is RideStatus.GoingToDestination)
                {
                    _mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value]++;
                }
                else if (userRole == UserType.Driver.ToString())
                {
                    _mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value]++;
                }

                if (_currentRideState.State is RideStatus.GoingToDestination or RideStatus.Finished
                    or RideStatus.DriverGoingToDestination)
                {
                    if (_stay <= 2)
                    {
                        _mockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value]--;
                        _stay++;
                    }
                }
                else
                {
                    _stay = 0;
                }

                await OnSuccessAsync(nextGeolocation);
                return true;
            }
        }

        return false;
    }

    private async ValueTask RequestGeoLocationAsync()
    {
        await RequestGeoLocationAsync(enableHighAccuracy: true, maximumAgeInMilliseconds: 0);
    }
}