using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Contracts.SignalRModels;

public class DriverAcceptRide
{
    public string InstanceId { get; set; }
    
    public Geolocation DestinationGeolocation { get; set; }
    
    public double Distance { get; set; }
}