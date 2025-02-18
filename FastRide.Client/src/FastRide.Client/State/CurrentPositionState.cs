using System;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.State;

public class CurrentPositionState
{
    private Geolocation _geolocation;
    
    public event Action OnChange;
    
    public Geolocation Geolocation
    {
        get => _geolocation;
        set
        {
            _geolocation = value;
            OnChange?.Invoke();
        }
    }
}