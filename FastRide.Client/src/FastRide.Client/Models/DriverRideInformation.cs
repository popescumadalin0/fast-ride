using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Models;

public class DriverRideInformation
{
    public string InstanceId { get; set; }

    public string Destination { get; set; }
    
    public Geolocation DestinationLocation { get; set; }
    
    public double Distance { get; set; }
    
    public UserIdentifier User { get; set; }
}