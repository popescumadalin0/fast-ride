using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Contracts.SignalRModels;

public class NotifyUserGeolocation
{
    public string UserId { get; set; }
    
    public Geolocation Geolocation { get; set; }
}