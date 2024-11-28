using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Snackbar;
using FastRide.Client.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Pages;

public partial class History
{
    private IEnumerable<IGrouping<string, RideInformation>> _rideGroups;
    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    private SnackbarStack SnackBar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await FastRideApiClient.GetRidesByUserAsync();
        if (!response.Success)
        {
            await SnackBar.PushAsync("Something went wrong",
                SnackbarColor.Warning,
                options => { options.IntervalBeforeClose = (double)Delay.Notification; });
            return;
        }

        _rideGroups = response.Response 
            .Select(x => new RideInformation()
            {
                FinishTime = x.FinishTime,
                Cost = x.Cost,
                Destination = x.Destination,
                Id = x.Id,
                DriverEmail = x.DriverEmail,
            })
            .GroupBy(x => x.FinishTime.ToString("dd MMM, HH:mm"));
    }
}

public enum Delay
{
    None = 0,
    Notification = 5000,
}