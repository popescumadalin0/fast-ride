using FastRide.Server.Contracts.Enums;
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
    
    public string AddressName { get; set; }
    
    public InternRideStatus Status { get; set; }
    
    public CompleteStatus CompleteStatus { get; set; }
}