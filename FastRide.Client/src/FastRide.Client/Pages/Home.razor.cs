using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase
{
    private GoogleMap _map;
    private MapOptions _mapOptions;

    [Inject] private IGeolocationService GeolocationService { get; set; }

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
            MapTypeId = MapTypeId.Roadmap
        };
    }

    private async Task AfterMapRender()
    {
        var bounds = await LatLngBounds.CreateAsync(_map.JsRuntime);
    }
}