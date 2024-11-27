using System;

namespace FastRide.Client.Models;

public class RideInformation
{
    public string Id { get; set; }
    public string Destination { get; set; }
    public DateTime FinishTime { get; set; }
    public double Cost { get; set; }
}