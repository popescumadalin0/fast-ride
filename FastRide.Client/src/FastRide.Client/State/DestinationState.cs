using System;
using FastRide.Client.Models;

namespace FastRide.Client.State;

public class DestinationState
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