using System;
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
    private GoogleMap _map;
    private MapOptions _mapOptions;

    [Inject] private IGeolocationService GeolocationService { get; set; }
    
    [Inject] private DestinationState DestinationState { get; set; }

    private string SelectedSearchValue { get; set; }

    private string SelectedAutoCompleteText { get; set; }

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
            ColorScheme = ColorScheme.Dark,
            RenderingType = RenderingType.Vector,
            StreetViewControlOptions = new StreetViewControlOptions()
            {
                Position = ControlPosition.RightTop
            },
            CameraControlOptions = new CameraControlOptions()
            {
                Position = ControlPosition.TopRight
            }
        };

        DestinationState.OnChange += StateHasChanged;
    }
    
    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    private async Task AfterMapRender()
    {
        var bounds = await LatLngBounds.CreateAsync(_map.JsRuntime);
        
        DestinationState.Geolocation = new Geolocation();
    }
}