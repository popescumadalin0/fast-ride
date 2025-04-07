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

    private readonly IFastRideApiClient _fastRideApiClient;

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private readonly DestinationState _destinationState;

    public CurrentRideState(IFastRideApiClient fastRideApiClient,
        AuthenticationStateProvider authenticationStateProvider, DestinationState destinationState)
    {
        _fastRideApiClient = fastRideApiClient;
        _authenticationStateProvider = authenticationStateProvider;
        _destinationState = destinationState;
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

            var lastRideStatus = rides.Response.MaxBy(x => x.TimeStamp);

            switch (lastRideStatus.Status)
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
                    InstanceId = lastRideStatus.Id;
                    if (authState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ==
                        UserType.User.ToString())
                    {
                        State = lastRideStatus.Status switch
                        {
                            InternRideStatus.NewRideAvailable => RideStatus.FindingDriver,
                            InternRideStatus.GoingToDestination => RideStatus.GoingToDestination,
                            InternRideStatus.GoingToUser => RideStatus.GoingToUser,
                            _ => State
                        };
                    }
                    else
                    {
                        switch (lastRideStatus.Status)
                        {
                            case InternRideStatus.NewRideAvailable:
                            {
                                State = RideStatus.NewRideAvailable;
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = lastRideStatus.StartPointLat,
                                    Longitude = lastRideStatus.StartPointLng
                                };
                                break;
                            }
                            case InternRideStatus.GoingToDestination:
                            {
                                State = RideStatus.DriverGoingToDestination;
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = lastRideStatus.DestinationLat,
                                    Longitude = lastRideStatus.DestinationLng
                                };
                                break;
                            }
                            case InternRideStatus.GoingToUser:
                            {
                                State = RideStatus.DriverGoingToUser;
                                _destinationState.Geolocation = new Geolocation()
                                {
                                    Latitude = lastRideStatus.StartPointLat,
                                    Longitude = lastRideStatus.StartPointLng
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
}