using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;

namespace FastRide.Client.Pages;

public partial class Home : ComponentBase, IDisposable, IBrowserViewportObserver
{
    private Dictionary<string, Geolocation> _drivers = new Dictionary<string, Geolocation>();

    [Inject] private ISignalRService SignalRService { get; set; } = default!;

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; } = default!;

    [Inject] private IConfiguration Configuration { get; set; } = default!;

    public Guid Id { get; } = Guid.NewGuid();

    private string _state;

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
        SignalRService.NotifyDriverGeolocation -= NotifyDriverGeolocationAsync;
    }

    protected override async Task OnInitializedAsync()
    {
        var currentPosition = await GeolocationService.GetGeolocationAsync();

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
        return InvokeAsync(StateHasChanged);
    }
}