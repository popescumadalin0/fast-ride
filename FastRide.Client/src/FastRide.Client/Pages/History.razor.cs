using System;
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
using Geolocation = FastRide.Client.Models.Geolocation;

namespace FastRide.Client.Pages;

public partial class History : IDisposable
{
    private IEnumerable<IGrouping<string, RideInformation>> _rideGroups;
    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; }

    [Inject] private OverlayState OverlayState { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

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

        response.Response.AddRange(new[]
        {
            new Ride()
            {
                TimeStamp = DateTime.Now,
                DestinationLat = 44.4647452,
                DestinationLng = 7.3553838,
                StartPointLat = 50.3,
                StartPointLng = 50.3,
                Driver = new UserIdentifier() { Email = "test@test.com" },
                Cost = 100.2,
                Id = Guid.NewGuid().ToString(),
            },
            new Ride()
            {
                TimeStamp = DateTime.Now,
                DestinationLat = 44.4647452,
                DestinationLng = 7.3553838,
                StartPointLat = 50.3,
                StartPointLng = 50.3,
                Driver = new UserIdentifier() { Email = "test@test.com" },
                Cost = 10.2,
                Id = Guid.NewGuid().ToString(),
            },
            new Ride()
            {
                TimeStamp = DateTime.Now,
                DestinationLat = 44.4647452,
                DestinationLng = 7.3553838,
                StartPointLat = 50.3,
                StartPointLng = 50.3,
                Driver = new UserIdentifier() { Email = "test@test.com" },
                Cost = 1.2,
                Id = Guid.NewGuid().ToString(),
            },
            new Ride()
            {
                TimeStamp = DateTime.Now.AddDays(-1),
                DestinationLat = 44.4647452,
                DestinationLng = 7.3553838,
                StartPointLat = 50.3,
                StartPointLng = 50.3,
                Driver = new UserIdentifier() { Email = "test@test.com" },
                Cost = 1.2,
                Id = Guid.NewGuid().ToString(),
            },
            new Ride()
            {
                TimeStamp = DateTime.Now.AddDays(-1),
                DestinationLat = 44.4647452,
                DestinationLng = 7.3553838,
                StartPointLat = 50.3,
                StartPointLng = 50.3,
                Driver = new UserIdentifier() { Email = "test@test.com" },
                Cost = 1.2,
                Id = Guid.NewGuid().ToString(),
            }
        });

        var rides = new List<RideInformation>();

        if (response.Response.Count != 0)
        {
            foreach (var ride in response.Response)
            {
                var destination =
                    await GeolocationService.GetAddressByLatLongAsync(ride.DestinationLat, ride.DestinationLng);
                rides.Add(new RideInformation()
                {
                    TimeStamp = ride.TimeStamp,
                    Cost = ride.Cost,
                    Destination = destination,
                    Id = ride.Id,
                    DriverEmail = ride.Driver.Email,
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

    private Task RebookClickedAsync(RideInformation ride)
    {
        DestinationState.Geolocation = ride.DestinationLocation;
        NavigationManager.NavigateTo("/");

        return Task.CompletedTask;
    }
}