using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Layout;

public partial class AcceptRidePopup : IDisposable
{
    private bool _openAvailableRide;

    private bool _opened;

    private RideInformation _ride;
    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        DestinationState.OnChange += StateHasChanged;

        await base.OnInitializedAsync();
    }

    private async Task OpenRideAsync()
    {
        _openAvailableRide = !_openAvailableRide;

        if (!_openAvailableRide)
        {
            var locationText = await LocationService.GetAddressByLatLongAsync(DestinationState.Geolocation.Latitude,
                DestinationState.Geolocation.Longitude);

            _ride =
                new RideInformation()
                {
                    Destination = locationText,
                };
        }

        StateHasChanged();
    }

    private async Task AcceptRideAsync()
    {
        _opened = false;
        await SignalRService.AcceptRideAsync(_ride);
    }
}