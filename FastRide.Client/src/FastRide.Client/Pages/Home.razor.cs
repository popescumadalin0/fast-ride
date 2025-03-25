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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

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

    private Geolocation _currentGeolocation;

    private readonly Dictionary<string, Geolocation> _drivers = new();

    private RealTimeMap _realTimeMap = new RealTimeMap();

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
        CurrentPositionState.OnChange -= CurrentPositionStateOnChange;
    }

    protected override async Task OnInitializedAsync()
    {
        _currentGeolocation = await GeolocationService.GetGeolocationAsync();

        DestinationState.OnChange += StateHasChanged;

        SignalRService.NotifyDriverGeolocation += NotifyDriverGeolocationAsync;

        CurrentPositionState.OnChange += CurrentPositionStateOnChange;

        StateHasChanged();
    }

    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        return ["test", "test1", "test2", "test3"];
    }

    private async Task NotifyDriverGeolocationAsync(NotifyUserGeolocation input)
    {
        var authState = await AuthenticationState;
        if (input.UserId != authState.User?.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value)
        {
            _drivers[input.UserId] = new Geolocation()
            {
                Longitude = input.Geolocation.Longitude,
                Latitude = input.Geolocation.Latitude,
            };
            await LoadDriversAsync();
        }
    }

    private void CurrentPositionStateOnChange()
    {
        _currentGeolocation = CurrentPositionState.Geolocation;
        LoadCurrentUser().GetAwaiter().GetResult();
    }

    private async Task MapLoadedAsync(RealTimeMap.MapEventArgs obj)
    {
        var realTimeMap = obj.sender;

        realTimeMap.Geometric.Points.changeExtentWhenAddPoints = false;
        realTimeMap.Geometric.Points.changeExtentWhenMovingPoints = false;
        realTimeMap.Parameters.zoomLevel = 18;
        
        realTimeMap.Options.keyboardNavigationOptions = new RealTimeMap.KeyboardNavigationOptions()
        {
            keyboard = false,
        };
        realTimeMap.Options.interactionOptions = new RealTimeMap.InteractionOptions()
        {
            dragging = false,
            trackResize = false,
            doubleClickZoom = true,
            shiftBoxZoom = false,
        };

        await PredefineMapContentAsync(realTimeMap);

        await LoadCurrentUser();

        await LoadDriversAsync();
    }

    private async Task MapClickedAsync(RealTimeMap.ClicksMapArgs obj)
    {
        var realTimeMap = obj.sender;

        await UpdatePointPositionAsync(realTimeMap, Configuration["Map:PinGuid"]!, "pin", new Geolocation()
        {
            Longitude = obj.location.longitude, Latitude = obj.location.latitude
        });

        DestinationState.Geolocation = new Geolocation()
        {
            Longitude = obj.location.longitude,
            Latitude = obj.location.latitude,
        };
    }

    private static Task PredefineMapContentAsync(RealTimeMap realTimeMap)
    {
        realTimeMap.Geometric.Points.Appearance(item => item.type == "current").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/currentCar.png", iconSize = [40, 40] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "pin").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/pin.png", iconSize = [40, 40] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "human").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/human.png", iconSize = [40, 40] };

        realTimeMap.Geometric.Points.Appearance(item => item.type == "driver").pattern =
            new RealTimeMap.PointIcon() { iconUrl = $"icons/driver.png", iconSize = [40, 40] };

        return Task.CompletedTask;
    }

    private async Task LoadCurrentUser()
    {
        //todo: if in ride
        var type = true ? "human" : "currentCar";

        var authState = await AuthenticationState;

        var id = authState.User.Identity?.IsAuthenticated ?? false
            ? authState.User.Claims.Single(x => x.Type == "sub").Value
            : Guid.Empty.ToString();

        await UpdatePointPositionAsync(_realTimeMap, id, type, _currentGeolocation);
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
            await UpdatePointPositionAsync(_realTimeMap, driver.Key, "driver", driver.Value);
        }
    }

    private async Task UpdatePointPositionAsync(RealTimeMap realTimeMap, string id, string type,
        Geolocation geolocation)
    {
        const double tolerance = 0.001;
        var removePoints = realTimeMap.Geometric.Points.getItems(x =>
            x.guid == Guid.Parse(id) && x.type == "pin" &&
            Math.Abs(x.latitude - geolocation.Latitude) < tolerance &&
            Math.Abs(x.longitude - geolocation.Longitude) < tolerance);

        if (removePoints.Count != 0)
        {
            await _realTimeMap.Geometric.Points.delete(removePoints.Select(x => x.guid.ToString()).ToArray());
        }

        var points = realTimeMap.Geometric.Points.getItems(x =>
            x.guid == Guid.Parse(id));

        if (points.Count != 0)
        {
            await _realTimeMap.Geometric.Points.update(
                new RealTimeMap.StreamPoint
                {
                    guid = Guid.Parse(id),
                    latitude = geolocation.Latitude,
                    longitude = geolocation.Longitude,
                    type = type
                }
            );
        }
        else
        {
            await _realTimeMap.Geometric.Points.add([
                new RealTimeMap.StreamPoint
                {
                    guid = Guid.Parse(id),
                    latitude = geolocation.Latitude,
                    longitude = geolocation.Longitude,
                    type = type
                }
            ]);
        }
    }
}