using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Contracts.SignalRModels;

public class NewRideInput
{
    public Geolocation Destination { get; set; }

    public Geolocation StartPoint { get; set; }

    public UserIdentifier User { get; set; }
}