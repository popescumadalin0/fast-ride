using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class GeolocationService : IGeolocationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<GeolocationService> _dotNetObjectReference;

    private AuthenticationStateProvider _authenticationStateProviderForMock;

    private event Func<Geolocation, ValueTask> CoordinatesChanged = default!;

    private event Func<GeolocationError, ValueTask> OnGeolocationPositionError = default!;

    public GeolocationService(IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProviderForMock)
    {
        _jsRuntime = jsRuntime;
        _authenticationStateProviderForMock = authenticationStateProviderForMock;

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

    private static readonly Dictionary<string, List<Geolocation>> Mocks = new Dictionary<string, List<Geolocation>>()
    {
        {
            "d69b1c1a-5abc-07fd-0581-42f8d305508b", [
                new Geolocation()
                {
                    Latitude = 44.289760,
                    Longitude = 23.807116
                },

                new Geolocation()
                {
                    Latitude = 44.289736,
                    Longitude = 23.807276
                },

                new Geolocation()
                {
                    Latitude = 44.289828,
                    Longitude = 23.807459
                },

                new Geolocation()
                {
                    Latitude = 44.289955,
                    Longitude = 23.807673
                },

                new Geolocation()
                {
                    Latitude = 44.290101,
                    Longitude = 23.807915
                },

                new Geolocation()
                {
                    Latitude = 44.290197,
                    Longitude = 23.808049
                },

                new Geolocation()
                {
                    Latitude = 44.290835,
                    Longitude = 23.808961
                },

                new Geolocation()
                {
                    Latitude = 44.291234,
                    Longitude = 23.809733
                },

                new Geolocation()
                {
                    Latitude = 44.291564,
                    Longitude = 23.810109
                },

                new Geolocation()
                {
                    Latitude = 44.291764,
                    Longitude = 23.810452
                },

                new Geolocation()
                {
                    Latitude = 44.291994,
                    Longitude = 23.810828
                },

                new Geolocation()
                {
                    Latitude = 44.292209,
                    Longitude = 23.811074
                },

                new Geolocation()
                {
                    Latitude = 44.292394,
                    Longitude = 23.811407
                },

                new Geolocation()
                {
                    Latitude = 44.292562,
                    Longitude = 23.811740
                },

                new Geolocation()
                {
                    Latitude = 44.292793,
                    Longitude = 23.811986
                },

                new Geolocation()
                {
                    Latitude = 44.292954,
                    Longitude = 23.812330
                },

                new Geolocation()
                {
                    Latitude = 44.293177,
                    Longitude = 23.812641
                },

                new Geolocation()
                {
                    Latitude = 44.293369,
                    Longitude = 23.812941
                },

                new Geolocation()
                {
                    Latitude = 44.293561,
                    Longitude = 23.813167
                },

                new Geolocation()
                {
                    Latitude = 44.293730,
                    Longitude = 23.813446
                },

                new Geolocation()
                {
                    Latitude = 44.293968,
                    Longitude = 23.813639
                }
            ]
        }
    };

    private static readonly Dictionary<string, int> MockIndexes = new Dictionary<string, int>()
    {
        { "d69b1c1a-5abc-07fd-0581-42f8d305508b", 0 }
    };

    private async ValueTask RequestGeoLocationAsync(bool enableHighAccuracy, int maximumAgeInMilliseconds)
    {
        var authState = await _authenticationStateProviderForMock.GetAuthenticationStateAsync();

        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            if (Mocks.TryGetValue(authState.User.Claims.First(x => x.Type == "sub").Value, out var value))
            {
                if (value.Count <= MockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value])
                {
                    MockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value] = 0;
                }

                await OnSuccessAsync(
                    value[
                        MockIndexes[authState.User.Claims.First(x => x.Type == "sub").Value]++]);
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