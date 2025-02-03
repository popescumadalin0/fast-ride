using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Server.Contracts.Models;
using Microsoft.JSInterop;

namespace FastRide.Client.Service;

public class GeolocationService : IGeolocationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<GeolocationService> _dotNetObjectReference;

    private event Func<Geolocation, ValueTask> CoordinatesChanged = default!;

    private event Func<GeolocationError, ValueTask> OnGeolocationPositionError = default!;

    public GeolocationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;

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

    private async ValueTask RequestGeoLocationAsync(bool enableHighAccuracy, int maximumAgeInMilliseconds)
    {
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