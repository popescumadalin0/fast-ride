using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Client.Service;
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

    public async Task UpdateState(Ride ride)
    {
        const double tolerance = 0.00005;

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        switch (ride.Status)
        {
            case InternRideStatus.Cancelled:
                State = RideStatus.Cancelled;
                break;
            case InternRideStatus.Finished:
                State = RideStatus.Finished;
                break;
            case InternRideStatus.None:
                State = RideStatus.None;
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
                    switch (ride.Status)
                    {
                        case InternRideStatus.NewRideAvailable:
                            State = RideStatus.FindingDriver;
                            if (_destinationState.Geolocation == null ||
                                Math.Abs(ride.DestinationLng - _destinationState.Geolocation.Longitude) > tolerance ||
                                Math.Abs(ride.DestinationLat - _destinationState.Geolocation.Latitude) > tolerance)
                            {
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = ride.DestinationLat,
                                    Longitude = ride.DestinationLng
                                };
                            }

                            break;
                        case InternRideStatus.GoingToDestination:
                            State = RideStatus.GoingToDestination;
                            if (_destinationState.Geolocation == null ||
                                Math.Abs(ride.DestinationLng - _destinationState.Geolocation.Longitude) > tolerance ||
                                Math.Abs(ride.DestinationLat - _destinationState.Geolocation.Latitude) > tolerance)
                            {
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = ride.DestinationLat,
                                    Longitude = ride.DestinationLng
                                };
                            }

                            break;
                        case InternRideStatus.GoingToUser:
                            State = RideStatus.GoingToUser;
                            if (_destinationState.Geolocation == null ||
                                Math.Abs(ride.DestinationLng - _destinationState.Geolocation.Longitude) > tolerance ||
                                Math.Abs(ride.DestinationLat - _destinationState.Geolocation.Latitude) > tolerance)
                            {
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = ride.DestinationLat,
                                    Longitude = ride.DestinationLng
                                };
                            }

                            break;
                        default:
                            State = State;
                            break;
                    }
                }
                else
                {
                    switch (ride.Status)
                    {
                        case InternRideStatus.NewRideAvailable:
                        {
                            State = RideStatus.NewRideAvailable;
                            if (_destinationState.Geolocation == null ||
                                Math.Abs(ride.StartPointLng - _destinationState.Geolocation.Longitude) > tolerance ||
                                Math.Abs(ride.StartPointLat - _destinationState.Geolocation.Latitude) > tolerance)
                            {
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = ride.StartPointLat,
                                    Longitude = ride.StartPointLng
                                };
                            }

                            break;
                        }
                        case InternRideStatus.GoingToDestination:
                        {
                            State = RideStatus.DriverGoingToDestination;
                            if (_destinationState.Geolocation == null ||
                                Math.Abs(ride.DestinationLng - _destinationState.Geolocation.Longitude) > tolerance ||
                                Math.Abs(ride.DestinationLat - _destinationState.Geolocation.Latitude) > tolerance)
                            {
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = ride.DestinationLat,
                                    Longitude = ride.DestinationLng
                                };
                            }

                            break;
                        }
                        case InternRideStatus.GoingToUser:
                        {
                            State = RideStatus.DriverGoingToUser;
                            if (_destinationState.Geolocation == null ||
                                Math.Abs(ride.StartPointLng - _destinationState.Geolocation.Longitude) > tolerance ||
                                Math.Abs(ride.StartPointLat - _destinationState.Geolocation.Latitude) > tolerance)
                            {
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = ride.StartPointLat,
                                    Longitude = ride.StartPointLng
                                };
                            }

                            break;
                        }
                        case InternRideStatus.Finished:
                        case InternRideStatus.Cancelled:
                        case InternRideStatus.None:
                        default:
                            InstanceId = null;
                            State = State;
                            break;
                    }
                }

                break;
            }
        }
    }
}