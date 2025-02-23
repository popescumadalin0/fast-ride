using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private CurrentPositionState CurrentPositionState { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; }

    [Inject] private IConfiguration Configuration { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }

    private string _state;

    private readonly Dictionary<string, Geolocation> _drivers = new();

    private RealTimeMap _realTimeMap = new RealTimeMap();

    private readonly RealTimeMap.LoadParameters _parameters = new()
    {
        zoomLevel = 18,
        basemap = new RealTimeMap.Basemap(),
    };

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
        CurrentPositionState.OnChange -= CurrentPositionStateOnChange;
    }

    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;

        SignalRService.NotifyDriverGeolocation += NotifyDriverGeolocationAsync;

        CurrentPositionState.OnChange += CurrentPositionStateOnChange;

        StateHasChanged();
    }

    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        return ["test", "test1", "test2", "test3"];
    }

    private async Task NotifyDriverGeolocationAsync(NotifyUserGeolocation geolocation)
    {
        _drivers[geolocation.UserId] = new Geolocation()
        {
            Longitude = geolocation.Geolocation.Longitude,
            Latitude = geolocation.Geolocation.Latitude,
        };

        await LoadDriversAsync();
    }

    private void CurrentPositionStateOnChange()
    {
        LoadCurrentUser().GetAwaiter().GetResult();
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

        DestinationState.Geolocation = new Geolocation()
        {
            Longitude = obj.location.longitude,
            Latitude = obj.location.latitude,
        };
    }

    private async Task MapLoadedAsync(RealTimeMap.MapEventArgs obj)
    {
        var realTimeMap = obj.sender;

        _realTimeMap = realTimeMap;

        _realTimeMap.Parameters.location = new RealTimeMap.Location()
        {
            latitude = CurrentPositionState.Geolocation.Latitude,
            longitude = CurrentPositionState.Geolocation.Longitude,
        };

        _realTimeMap.initialization();

        await PredefineMapContentAsync(realTimeMap);

        await LoadCurrentUser();

        await LoadDriversAsync();
    }

    private Task PredefineMapContentAsync(RealTimeMap realTimeMap)
    {
        realTimeMap.Geometric.Points.Appearance(item => item.type == "current").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/currentCar.png", iconSize = [32, 32] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "pin").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/pin.png", iconSize = [32, 32] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "human").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/human.png", iconSize = [32, 32] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "driver").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/driver.png", iconSize = [32, 32] };

        return Task.CompletedTask;
    }

    private async Task LoadCurrentUser()
    {
        //todo: if in ride
        var type = true ? "human" : "currentCar";

        var authState = await AuthenticationState;

        var id = authState.User.Identity!.IsAuthenticated
            ? authState.User.Claims.Single(x => x.Type == "sub").Value
            : Guid.Empty.ToString();

        await _realTimeMap.Geometric.Points.delete(id);

        await _realTimeMap.Geometric.Points.upload([
            new RealTimeMap.StreamPoint
            {
                guid = Guid.Parse(id),
                latitude = CurrentPositionState.Geolocation.Latitude,
                longitude = CurrentPositionState.Geolocation.Longitude,
                type = type
            }
        ]);
    }

    private async Task LoadDriversAsync()
    {
        //todo: if in ride
        if (false)
        {
            return;
        }

        foreach (var driver in _drivers)
        {
            await _realTimeMap.Geometric.Points.delete(driver.Key);

            await _realTimeMap.Geometric.Points.upload([
                new RealTimeMap.StreamPoint()
                {
                    guid = Guid.Parse(driver.Key),
                    latitude = driver.Value.Latitude,
                    longitude = driver.Value.Longitude,
                    type = "driver"
                }
            ]);
        }
    }
}