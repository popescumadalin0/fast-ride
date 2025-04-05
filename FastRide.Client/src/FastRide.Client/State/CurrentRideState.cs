using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.State;

public class CurrentRideState : ICurrentRideState
{
    private RideStatus _state;

    private readonly IFastRideApiClient _fastRideApiClient;

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CurrentRideState(IFastRideApiClient fastRideApiClient,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _fastRideApiClient = fastRideApiClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public event Action OnChange;

    public RideStatus State
    {
        get => _state;
        private set
        {
            _state = value;
            OnChange?.Invoke();
        }
    }

    public void ResetState()
    {
        State = RideStatus.None;
    }

    public async Task UpdateState()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            var rides = await _fastRideApiClient.GetRidesByUserAsync();

            if (!rides.Success)
            {
                throw new Exception(
                    $"Failed to get rides. {rides.ClientError} - {rides.ReasonPhrase} - {rides.ResponseMessage}");
            }

            if (rides.Response.Count == 0)
            {
                State = RideStatus.None;
                return;
            }

            var lastRideStatus = rides.Response.MaxBy(x => x.TimeStamp).Status;

            switch (lastRideStatus)
            {
                case InternRideStatus.Cancelled:
                    State = RideStatus.Cancelled;
                    break;
                case InternRideStatus.None:
                case InternRideStatus.Finished:
                    State = RideStatus.Finished;
                    break;
                case InternRideStatus.NewRideAvailable:
                case InternRideStatus.GoingToUser:
                case InternRideStatus.GoingToDestination:
                default:
                {
                    if (authState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ==
                        UserType.User.ToString())
                    {
                        State = lastRideStatus switch
                        {
                            InternRideStatus.NewRideAvailable => RideStatus.FindingDriver,
                            InternRideStatus.GoingToDestination => RideStatus.GoingToDestination,
                            InternRideStatus.GoingToUser => RideStatus.GoingToUser,
                            _ => State
                        };
                    }
                    else
                    {
                        State = lastRideStatus switch
                        {
                            InternRideStatus.NewRideAvailable => RideStatus.NewRideAvailable,
                            InternRideStatus.GoingToDestination => RideStatus.DriverGoingToDestination,
                            InternRideStatus.GoingToUser => RideStatus.DriverGoingToUser,
                            _ => State
                        };
                    }

                    break;
                }
            }
        }
    }
}