using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.State;

public class DestinationState
{
    private Geolocation _geolocation;
    
    public event Func<Task> OnChange;
    
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