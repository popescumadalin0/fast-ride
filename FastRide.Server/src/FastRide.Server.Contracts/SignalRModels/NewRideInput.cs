using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Contracts.SignalRModels;

public class NewRideInput
{
    public Geolocation Destination { get; set; }

    public Geolocation StartPoint { get; set; }

    public UserIdentifier User { get; set; }
    
    public UserIdentifier Driver { get; set; }

    public string GroupName { get; set; }
    
    public double Cost  { get; set; }
}