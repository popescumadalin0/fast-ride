using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Models;

public class SendPriceCalculationActivityInput
{
    public Geolocation Destination { get; set; }
    
    public Geolocation StartPoint { get; set; }
    public string InstanceId { get; set; }
    
    public string UserId { get; set; }
}