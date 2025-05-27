using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace FastRide.Client.Pages;

public partial class Map : ComponentBase, IAsyncDisposable
{
    [Parameter] public Action<Geolocation> OnClickMap { get; set; }

    [Parameter] public Func<Task> OnMapRendered { get; set; }
    
    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; }
    
    [Inject] private ILocationService LocationService { get; set; }

    private Dictionary<string, Geolocation> _drivers = new ();
    
    public string Locality { get; set; }
    
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
            var currentGeolocation = await GeolocationService.GetGeolocationAsync();

            _dotNetObjectReference = DotNetObjectReference.Create(this);
            
            var auth = await AuthenticationState;
            var userId = auth.User.Identity?.IsAuthenticated ?? false
                ? auth.User.Claims.Single(x => x.Type == "sub").Value
                : Guid.Empty.ToString();

            await JsRuntime.InvokeVoidAsync(
                "window.leafletInitMap",
                _dotNetObjectReference,
                userId,
                currentGeolocation.Latitude,
                currentGeolocation.Longitude,
                _assets["human"],
                18,
                nameof(OnClickMapEvent));

            Locality = await LocationService.GetLocalityByLatLongAsync(currentGeolocation.Latitude,
                currentGeolocation.Longitude);

            if (OnClickMap != null)
            {
                await OnMapRendered.Invoke();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task SetUserLocationAsync(string userId, Geolocation geolocation, bool inCar)
    {
        if (inCar)
        {
            await JsRuntime.InvokeVoidAsync("window.leafletAddUser", userId, geolocation.Latitude,
                geolocation.Longitude, _assets["currentCar"]);
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("window.leafletAddUser", userId, geolocation.Latitude,
                geolocation.Longitude, _assets["human"]);
        }
    }

    public async Task SetDriverLocationAsync(string driverId, Geolocation geolocation)
    {
        _drivers[driverId] = new Geolocation()
        {
            Longitude = geolocation.Longitude,
            Latitude = geolocation.Latitude,
        };

        foreach (var driver in _drivers)
        {
            await JsRuntime.InvokeVoidAsync("window.leafletAddUser", driver.Key, driver.Value.Latitude,
                driver.Value.Longitude, _assets["driver"]);
        }
    }

    public async Task SetPinLocationAsync(Geolocation location)
    {
        var setValue = location == null
            ? null
            : new
            {
                latlng = new LeafletGeolocation()
                {
                    lat = location.Latitude,
                    lng = location.Longitude
                }
            };

        await JsRuntime.InvokeVoidAsync("window.leafletSetPinLocation", setValue);
    }
    
    public async Task DrawRouteAsync(Geolocation start, Geolocation end)
    {
        await JsRuntime.InvokeVoidAsync("window.leafletDrawRoute", start.Latitude, start.Longitude, end.Latitude,
            end.Longitude);
    }
    
    public async Task RemoveRouteAsync()
    {
        await JsRuntime.InvokeVoidAsync("window.leafletRemoveRoute");
    }

    [JSInvokable]
    public void OnClickMapEvent(LeafletGeolocation geolocation)
    {
        if (OnClickMap != null)
        {
            if (geolocation == null)
            {
                OnClickMap.Invoke(null);
            }
            else
            {
                OnClickMap.Invoke(new Geolocation()
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