namespace FastRide.Server.Contracts.Models;

public class StartRide
{
    public Geolocation Destination { get; set; }

    public Geolocation StartPoint { get; set; }

    public UserIdentifier User { get; set; }
}