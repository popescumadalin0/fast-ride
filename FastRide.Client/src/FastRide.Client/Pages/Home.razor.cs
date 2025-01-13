using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase, IDisposable
{
    private Dictionary<string, Geolocation> _drivers = new Dictionary<string, Geolocation>();

    private GoogleMap _map;
    private MapOptions _mapOptions;

    private string _state;

    [Inject] private ISignalRService SignalRService { get; set; } = default!;

    [Inject] private DestinationState DestinationState { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
    }

    protected override async Task OnInitializedAsync()
    {
        var currentPosition = new Geolocation()
        {
            Longitude = 30.0,
            Latitude = 100.0,
        };
        //currentPosition = await GeolocationService.GetLocationAsync();
        _mapOptions = new MapOptions()
        {
            Zoom = 13,
            Center = new LatLngLiteral()
            {
                Lat = 30.0, //currentPosition.Latitude,
                Lng = 100.0, //currentPosition.Longitude
            },
            FullscreenControl = false,
            MapTypeId = MapTypeId.Roadmap,
            MapTypeControl = false,
            ZoomControl = false,
            StreetViewControl = false,
            ColorScheme = ColorScheme.Dark,
            CameraControl = false
        };

        DestinationState.OnChange += StateHasChanged;

        SignalRService.NotifyDriverGeolocation += NotifyDriverGeolocationAsync;
    }

    private async Task AfterMapRender()
    {
        var bounds = await LatLngBounds.CreateAsync(_map.JsRuntime);
    }

    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        return ["test", "test1", "test2", "test3"];
    }

    private Task NotifyDriverGeolocationAsync(string userId, Geolocation geolocation)
    {
        _drivers[userId] = new Geolocation()
        {
            Longitude = geolocation.Longitude,
            Latitude = geolocation.Latitude,
        };
        return Task.CompletedTask;
    }
}