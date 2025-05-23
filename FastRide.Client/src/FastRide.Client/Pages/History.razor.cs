﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Pages;

public partial class History : IDisposable
{
    private IEnumerable<IGrouping<string, RideInformation>> _rideGroups;
    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private ILocationService LocationService { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; }

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
            SnackBar.Add("Something went wrong", Severity.Error);
            return;
        }

        var rides = new List<RideInformation>();

        if (response.Response.Count != 0)
        {
            foreach (var ride in response.Response)
            {
                var destination =
                    await LocationService.GetAddressByLatLongAsync(ride.DestinationLat, ride.DestinationLng);
                rides.Add(new RideInformation()
                {
                    TimeStamp = ride.TimeStamp,
                    Cost = ride.Cost,
                    Destination = destination,
                    Id = ride.Id,
                    DestinationLocation = new Geolocation()
                    {
                        Longitude = ride.DestinationLng,
                        Latitude = ride.DestinationLat
                    },
                });
            }

            _rideGroups = rides.GroupBy(x => x.TimeStamp.ToString("dd MMM, HH:mm"));
        }

        OverlayState.DataLoading = false;
    }
}