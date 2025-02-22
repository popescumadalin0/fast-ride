using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using LeafletForBlazor;
using LeafletForBlazor.Components;
using LeafletForBlazor.RealTime.geometry;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; } = default!;

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; } = default!;

    [Inject] private IConfiguration Configuration { get; set; } = default!;

    public Guid Id { get; } = Guid.NewGuid();

    private string _state;

    private readonly Dictionary<string, Geolocation> _drivers = new();

    private RealTimeMap _realTimeMap;

    private Geolocation _currentPosition;

    private readonly RealTimeMap.LoadParameters _parameters = new()
    {
        zoomLevel = 18,
        basemap = new RealTimeMap.Basemap(),
    };

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
    }

    protected override async Task OnInitializedAsync()
    {
        _currentPosition = await GeolocationService.GetGeolocationAsync();

        _realTimeMap.Parameters.location = new RealTimeMap.Location()
        {
            latitude = _currentPosition.Latitude,
            longitude = _currentPosition.Longitude,
        };

        _realTimeMap.initialization();

        DestinationState.OnChange += StateHasChanged;

        SignalRService.NotifyDriverGeolocation += NotifyDriverGeolocationAsync;

        StateHasChanged();
    }

    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        return ["test", "test1", "test2", "test3"];
    }

    private Task NotifyDriverGeolocationAsync(NotifyUserGeolocation geolocation)
    {
        _drivers[geolocation.UserId] = new Geolocation()
        {
            Longitude = geolocation.Geolocation.Longitude,
            Latitude = geolocation.Geolocation.Latitude,
        };
        return Task.CompletedTask;
    }

    private async Task MapClickedAsync(RealTimeMap.ClicksMapArgs obj)
    {
        var realTimeMap = obj.sender;

        await realTimeMap.Geometric.Points.delete(Configuration["Map:PinGuid"]!);

        await realTimeMap.Geometric.Points.upload(new List<RealTimeMap.StreamPoint>()
        {
            new()
            {
                guid = Guid.Parse(Configuration["Map:PinGuid"]!),
                latitude = obj.location.latitude,
                longitude = obj.location.longitude,
                type = "pin"
            },
        });
    }

    private async Task MapLoadedAsync(RealTimeMap.MapEventArgs obj)
    {
        var realTimeMap = obj.sender;
        realTimeMap.Geometric.Points.Appearance(item => item.type == "current").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/currentCar.png", iconSize = [32, 32] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "pin").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/pin.png", iconSize = [32, 32] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "human").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/human.png", iconSize = [32, 32] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "driver").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/driver.png", iconSize = [32, 32] };

        await realTimeMap.Geometric.Points.upload([
            new RealTimeMap.StreamPoint
            {
                guid = Guid.NewGuid(),
                latitude = _currentPosition.Latitude,
                longitude = _currentPosition.Longitude,
                type = "human"
            }
        ]);
    }
}