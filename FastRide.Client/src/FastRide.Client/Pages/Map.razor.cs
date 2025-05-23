using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FastRide.Client.Pages;

public partial class Map : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; }

    [Parameter] public Dictionary<string, Geolocation> Drivers { get; set; }

    [Parameter] public Geolocation CurrentGeolocation { get; set; }

    [Parameter] public Action<Geolocation> OnClickMap { get; set; }

    [Parameter] public bool InCar { get; set; }

    [Parameter] public string UserId { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    public string Locality { get; set; }

    private bool isInitialized;

    private Dictionary<string, string> _assets = new()
    {
        { "driver", "icons/driver.png" },
        { "human", "icons/human.png" },
        { "currentCar", "icons/currentCar.png" },
        { "pin", "icons/pin.png" }
    };

    private DotNetObjectReference<Map> _dotNetObjectReference;

    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("window.leafletDispose");
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CurrentGeolocation = await GeolocationService.GetGeolocationAsync();

            _dotNetObjectReference = DotNetObjectReference.Create(this);

            await JsRuntime.InvokeVoidAsync(
                "window.leafletInitMap",
                _dotNetObjectReference,
                CurrentGeolocation.Latitude,
                CurrentGeolocation.Longitude,
                18,
                nameof(OnClickMapEvent));

            Locality = await LocationService.GetLocalityByLatLongAsync(CurrentGeolocation.Latitude,
                CurrentGeolocation.Longitude);

            isInitialized = true;
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if(!isInitialized)
        {
            return;
        }

        if (InCar)
        {
            await JsRuntime.InvokeVoidAsync("window.leafletAddUser", UserId, CurrentGeolocation.Latitude,
                CurrentGeolocation.Longitude, _assets["currentCar"]);
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("window.leafletAddUser", UserId, CurrentGeolocation.Latitude,
                CurrentGeolocation.Longitude, _assets["human"]);
        }

        foreach (var driver in Drivers)
        {
            await JsRuntime.InvokeVoidAsync("window.leafletAddUser", driver.Key, driver.Value.Latitude,
                driver.Value.Longitude, _assets["driver"]);
        }

        await base.OnParametersSetAsync();
    }

    [JSInvokable]
    public void OnClickMapEvent(LeafletGeolocation geolocation)
    {
        if (OnClickMap != null)
        {
            if (geolocation == null)
            {
                OnClickMap?.Invoke(null);
            }
            else
            {
                OnClickMap?.Invoke(new Geolocation()
                {
                    Latitude = geolocation.lat,
                    Longitude = geolocation.lng
                });
            }
        }
    }
}

public class LeafletGeolocation
{
    public double lat { get; set; }

    public double lng { get; set; }
}