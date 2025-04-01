using System;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.State;

public class CurrentRideState
{
    private bool _inRide;
    
    public event Action OnChange;
    
    public bool InRide
    {
        get => _inRide;
        set
        {
            _inRide = value;
            OnChange?.Invoke();
        }
    }
}