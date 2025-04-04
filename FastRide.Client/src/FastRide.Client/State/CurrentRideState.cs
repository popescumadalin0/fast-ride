using System;
using FastRide.Client.Enums;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.State;

public class CurrentRideState
{
    private RideState _state;

    public event Action OnChange;

    public RideState State
    {
        get => _state;
        set
        {
            _state = value;
            OnChange?.Invoke();
        }
    }
}