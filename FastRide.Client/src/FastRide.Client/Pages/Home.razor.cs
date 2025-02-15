using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Majorsoft.Blazor.Components.Core.Extensions;
using Majorsoft.Blazor.Components.Maps;
using Majorsoft.Blazor.Components.Maps.Google;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase, IDisposable, IBrowserViewportObserver
{
    private Dictionary<string, Geolocation> _drivers = new Dictionary<string, Geolocation>();

    private string _state;

    [Inject] private ISignalRService SignalRService { get; set; } = default!;

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; } = default!;

    [Inject] private IConfiguration Configuration { get; set; } = default!;

    private string _googleMapsApiKey = "AIzaSyCShVblwf92noaROeZpDDxjKRy91WlnjYQ";

    public Guid Id { get; } = Guid.NewGuid();

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    //Javascript Maps
    private GoogleMap _googleMap;
    private GeolocationData _jsMapCenter;
    private string _jsMapBackgroundColor = "lightblue";
    private int _jsMapControlSize = 38;
    private byte _jsMapZoomLevel = 12;
    private int _jsMapWidth = 450;
    private int _jsMapHeight = 300;

    private GoogleMapTypes _jsMapType = GoogleMapTypes.Roadmap;
    private byte _jsTilt = 0;
    private GoogleMapControlPositions _jsFullscreenControlPositon = GoogleMapControlPositions.TOP_RIGHT;
    private GoogleMapGestureHandlingTypes _jsGestureHandling = GoogleMapGestureHandlingTypes.Auto;

    private GoogleMapTypeControlOptions _jsMapTypeControlOptions = new GoogleMapTypeControlOptions()
    {
        MapTypeControlStyle = GoogleMapTypeControlStyles.DROPDOWN_MENU,
    };

    private List<GoogleMapCustomControl> _jsCustomControls = new List<GoogleMapCustomControl>();
    private ObservableRangeCollection<GoogleMapMarker> _jsMarkers = new ObservableRangeCollection<GoogleMapMarker>();

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
    }

    protected override async Task OnInitializedAsync()
    {
        var currentPosition = await GeolocationService.GetGeolocationAsync();

        _jsMapCenter = new GeolocationData(currentPosition.Latitude, currentPosition.Longitude);

        _googleMapsApiKey = Configuration["GoogleMaps:ApiKey"];

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

    public Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        _jsMapWidth = browserViewportEventArgs.BrowserWindowSize.Width;
        _jsMapHeight = browserViewportEventArgs.BrowserWindowSize.Height;

        return InvokeAsync(StateHasChanged);
    }
}