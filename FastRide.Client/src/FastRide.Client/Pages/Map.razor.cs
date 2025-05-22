using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FastRide.Client.Pages;

public partial class Map : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; }

    private Geolocation _currentGeolocation;


    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("window.leafletDispose", _currentGeolocation.Latitude, _currentGeolocation.Longitude,
            18);
    }

    protected override async Task OnInitializedAsync()
    {
        _currentGeolocation = await GeolocationService.GetGeolocationAsync();

        await JsRuntime.InvokeVoidAsync("window.leafletInitMap", _currentGeolocation.Latitude, _currentGeolocation.Longitude,
            18);

        StateHasChanged();
    }
}