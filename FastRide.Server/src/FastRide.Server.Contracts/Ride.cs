using System;

namespace FastRide.Server.Contracts;

public class Ride
{
    public string Id { get; set; }

    public double DestinationLng { get; set; }

    public double DestinationLat { get; set; }

    public double StartPointLat { get; set; }

    public double StartPointLng { get; set; }

    public DateTime TimeStamp { get; set; }

    public double Cost { get; set; }

    public string DriverEmail { get; set; }

    public string UserEmail { get; set; }

    public RideStatus Status { get; set; }
}