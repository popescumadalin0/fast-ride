using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.State;

public class CurrentRideState : ICurrentRideState
{
    public string InstanceId { get; set; }

    private RideStatus _state;

    private readonly DestinationState _destinationState;

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CurrentRideState(DestinationState destinationState, AuthenticationStateProvider authenticationStateProvider)
    {
        _destinationState = destinationState;
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

    public async Task UpdateState(Ride ride)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        switch (ride.Status)
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
                InstanceId = ride.Id;
                if (authState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ==
                    UserType.User.ToString())
                {
                    State = ride.Status switch
                    {
                        InternRideStatus.NewRideAvailable => RideStatus.FindingDriver,
                        InternRideStatus.GoingToDestination => RideStatus.GoingToDestination,
                        InternRideStatus.GoingToUser => RideStatus.GoingToUser,
                        _ => State
                    };
                }
                else
                {
                    switch (ride.Status)
                    {
                        case InternRideStatus.NewRideAvailable:
                        {
                            State = RideStatus.NewRideAvailable;
                            _destinationState.Geolocation = new Geolocation()
                            {
                                Latitude = ride.StartPointLat,
                                Longitude = ride.StartPointLng
                            };
                            break;
                        }
                        case InternRideStatus.GoingToDestination:
                        {
                            State = RideStatus.DriverGoingToDestination;
                            _destinationState.Geolocation = new Geolocation()
                            {
                                Latitude = ride.DestinationLat,
                                Longitude = ride.DestinationLng
                            };
                            break;
                        }
                        case InternRideStatus.GoingToUser:
                        {
                            State = RideStatus.DriverGoingToUser;
                            _destinationState.Geolocation = new Geolocation()
                            {
                                Latitude = ride.StartPointLat,
                                Longitude = ride.StartPointLng
                            };
                            break;
                        }
                        case InternRideStatus.None:
                        case InternRideStatus.Finished:
                        case InternRideStatus.Cancelled:
                        default:
                            State = State;
                            break;
                    }
                }

                break;
            }
        }
    }
}