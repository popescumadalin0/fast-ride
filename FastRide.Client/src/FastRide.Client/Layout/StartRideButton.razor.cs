using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Components;
using FastRide.Client.Contracts;
using FastRide.Client.State;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Geolocation = FastRide.Server.Contracts.Models.Geolocation;

namespace FastRide.Client.Layout;

public partial class StartRideButton : IDisposable
{
    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private ISignalRService SignalRService { get; set; }

    [Inject] private DestinationState DestinationState { get; set; }

    [Inject] private IGeolocationService GeolocationService { get; set; }

    [CascadingParameter] private AuthenticationState AuthenticationState { get; set; }

    public void Dispose()
    {
        DestinationState.OnChange -= StateHasChanged;

        SignalRService.RideCreated -= RemoveUserFromGroups;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DestinationState.OnChange += StateHasChanged;

        SignalRService.RideCreated += RemoveUserFromGroups;
    }

    private async Task RideAsync()
    {
        var groupName = AuthenticationState.User.Claims.First(c => c.Type == ClaimTypes.GroupSid).Value;
        var email = AuthenticationState.User.Claims.First(c => c.Type == "email").Value;
        var userId = AuthenticationState.User.Claims.First(c => c.Type == "sub").Value;

        var currentLocation = await GeolocationService.GetCoordonatesAsync();

        await SignalRService.CreateNewRideAsync(groupName, new NewRideInput()
        {
            Destination = new Geolocation()
            {
                Longitude = DestinationState.Geolocation.Longitude,
                Latitude = DestinationState.Geolocation.Latitude,
            },
            StartPoint = new Geolocation()
            {
                Latitude = currentLocation.Latitude,
                Longitude = currentLocation.Longitude
            },
            User = new UserIdentifier()
            {
                Email = email,
                NameIdentifier = userId
            }
        });
    }

    private async Task RemoveUserFromGroups(string instanceId)
    {
        //todo: this when the payment is made
        /*var groupName = AuthenticationState.User.Claims.First(c => c.Type == ClaimTypes.GroupSid).Value;
        var userId = AuthenticationState.User.Claims.First(c => c.Type == "sub").Value;
        await Sender.RemoveUserFromGroupAsync(userId, groupName);*/

        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<PaymentConfirmationDialog>("Pay", options);
    }
}