using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace FastRide.Client.Pages;

public partial class History : IDisposable
{
    private IEnumerable<IGrouping<string, RideInformation>> _rideGroups;
    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; }

    [Inject] IJSRuntime JsRuntime { get; set; }

    [Inject] private OverlayState OverlayState { get; set; }

    public void Dispose()
    {
        OverlayState.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        OverlayState.OnChange += StateHasChanged;

        OverlayState.DataLoading = true;
        var response = await FastRideApiClient.GetRidesByUserAsync();
        if (!response.Success)
        {
            SnackBar.Add($"Something went wrong: {response.ResponseMessage}", Severity.Error);
            OverlayState.DataLoading = false;
            return;
        }

        if (response.Response.Count != 0)
        {
            var rideTasks = response.Response.OrderByDescending(x => x.TimeStamp).Select(async ride => new RideInformation
            {
                TimeStamp = await ConvertToLocalTimeZone(ride.TimeStamp),
                Cost = ride.Cost,
                Destination = ride.AddressName,
                Id = ride.Id,
                DestinationLocation = new Geolocation
                {
                    Longitude = ride.DestinationLng,
                    Latitude = ride.DestinationLat
                },
                CompleteStatus = ride.CompleteStatus
            });

            var rides = (await Task.WhenAll(rideTasks)).ToList();

            _rideGroups = rides.GroupBy(x => x.TimeStamp.Split(",")[0]);
        }

        OverlayState.DataLoading = false;
    }

    private async Task<string> ConvertToLocalTimeZone(DateTime utcDate)
    {
        var localDateString = await JsRuntime.InvokeAsync<string>(
            "toLocalTimeString",
            utcDate.ToString("o") // format ISO 8601
        );

        return localDateString;
    }
}