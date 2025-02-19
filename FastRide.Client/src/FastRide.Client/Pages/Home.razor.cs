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

    private async Task MapLoadedAsync(RealTimeMap.MapEventArgs obj)
    {
        var realTimeMap = obj.sender as RealTimeMap;
        realTimeMap.Geometric.Points.Appearance(item => item.type == "current user").pattern =
            new RealTimeMap.PointIcon()
            {
                shadowSize = [1,1,0,0],
                iconUrl = "car",
            };
        realTimeMap.Geometric.Points.Appearance(item => item.type != "asked for help").pattern =
            new RealTimeMap.PointTooltip()
            {
                content = "<b>${type}</b><br><b>Vehicle type: </b>${value.vehicleType}"
            };
        await realTimeMap.Geometric.Points.upload(new List<RealTimeMap.StreamPoint>()
        {
            new()
            {
                guid = Guid.NewGuid(),
                latitude = _currentPosition.Latitude,
                longitude = _currentPosition.Longitude,
                type = "current user",
                value = "current user",
            }
        });
    }
}