using System;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.State;

public class CurrentRideState
{
    private bool _inRide;

    private string _state;
    
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
    
    public string State
    {
        get => _state;
        set
        {
            _state = value;
            OnChange?.Invoke();
        }
    }
}