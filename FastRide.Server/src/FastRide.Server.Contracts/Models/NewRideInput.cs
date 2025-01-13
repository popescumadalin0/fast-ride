namespace FastRide.Server.Contracts.Models;

public class NewRideInput
{
    public Geolocation Destination { get; set; }

    public Geolocation StartPoint { get; set; }

    public UserIdentifier User { get; set; }
}