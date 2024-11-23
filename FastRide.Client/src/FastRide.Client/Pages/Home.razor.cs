using System.Threading.Tasks;
using FastRide.Client.Contracts;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase
{
    private GoogleMap _map;
    private MapOptions _mapOptions;

    [Inject] private IGeolocationService GeolocationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var currentPosition = await GeolocationService.GetLocationAsync();
        _mapOptions = new MapOptions()
        {
            Zoom = 13,
            Center = new LatLngLiteral()
            {
                Lat = currentPosition.Latitude,
                Lng = currentPosition.Longitude
            },
            MapTypeId = MapTypeId.Roadmap
        };
    }

    private async Task AfterMapRender()
    {
        var bounds = await LatLngBounds.CreateAsync(_map.JsRuntime);
    }
}