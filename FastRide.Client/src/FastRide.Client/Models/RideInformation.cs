using System;

namespace FastRide.Client.Models;

public class RideInformation
{
    public string Id { get; set; }
    public string Destination { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
    public double Cost { get; set; }

    public string DriverEmail { get; set; }
}