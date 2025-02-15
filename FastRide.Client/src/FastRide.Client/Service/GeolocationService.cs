using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using Majorsoft.Blazor.Components.Common.JsInterop.Geo;
using Microsoft.JSInterop;
using GeolocationError = FastRide.Client.Models.GeolocationError;
using IGeolocationService = FastRide.Client.Contracts.IGeolocationService;

namespace FastRide.Client.Service;

public class GeolocationService : IGeolocationService
{
    //private readonly IJSRuntime _jsRuntime;
    private Majorsoft.Blazor.Components.Common.JsInterop.Geo.IGeolocationService _geolocationService;
    private readonly DotNetObjectReference<GeolocationService> _dotNetObjectReference;

    private event Func<GeolocationResult, ValueTask> CoordinatesChanged = default!;

    private event Func<GeolocationError, ValueTask> OnGeolocationPositionError = default!;

    public GeolocationService(/*IJSRuntime jsRuntime,*/ Majorsoft.Blazor.Components.Common.JsInterop.Geo.IGeolocationService geolocationService)
    {
        //_jsRuntime = jsRuntime;
        _geolocationService = geolocationService;

        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    public async ValueTask<Geolocation> GetGeolocationAsync()
    {
        var tcs = new TaskCompletionSource<GeolocationResult>();

        Func<GeolocationResult, ValueTask> coordinatesChangedHandler = null;
        coordinatesChangedHandler = (geolocation) =>
        {
            tcs.SetResult(geolocation);

            CoordinatesChanged -= coordinatesChangedHandler;

            return ValueTask.CompletedTask;
        };

        CoordinatesChanged += coordinatesChangedHandler;

        await RequestGeoLocationAsync();

        var geolocation = await tcs.Task;

        if (geolocation.IsSuccess)
        {
            return new Geolocation()
            {
                Latitude = geolocation.Coordinates!.Latitude,
                Longitude = geolocation.Coordinates.Longitude,
            };
        }

        throw new Exception($"Failed to get geolocation: {geolocation.Error.ErrorMessage}");
        //return geolocation;
    }

    [JSInvokable]
    public async Task OnSuccessAsync(GeolocationResult coordinates)
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

    private async ValueTask RequestGeoLocationAsync(bool enableHighAccuracy, int maximumAgeInMilliseconds)
    {
        await _geolocationService.GetCurrentPositionAsync(OnSuccessAsync);
        /*await _jsRuntime.InvokeVoidAsync("window.getGeolocation",
            _dotNetObjectReference,
            enableHighAccuracy,
            maximumAgeInMilliseconds);*/
    }

    private async ValueTask RequestGeoLocationAsync()
    {
        await RequestGeoLocationAsync(enableHighAccuracy: true, maximumAgeInMilliseconds: 0);
    }
}