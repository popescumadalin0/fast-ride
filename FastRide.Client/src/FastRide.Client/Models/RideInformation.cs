using System;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Models;

public class RideInformation
{
    public string Id { get; set; }
    public string Destination { get; set; }

    public Geolocation DestinationLocation { get; set; }

    public string TimeStamp { get; set; }

    public double Cost { get; set; }
    
    public CompleteStatus CompleteStatus { get; set; }
}