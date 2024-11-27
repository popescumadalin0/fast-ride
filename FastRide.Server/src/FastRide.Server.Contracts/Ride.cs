using System;

namespace FastRide.Server.Contracts;

public class Ride
{
    public string Id { get; set; }
    public string Destination { get; set; }
    public DateTime FinishTime { get; set; }
    public double Cost { get; set; }
    
    public string DriverEmail { get; set; }
}