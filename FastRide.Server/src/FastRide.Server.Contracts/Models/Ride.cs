using System;
using FastRide.Server.Contracts.Enums;

namespace FastRide.Server.Contracts.Models;

public class Ride
{
    public string Id { get; set; }

    public double DestinationLng { get; set; }

    public double DestinationLat { get; set; }

    public double StartPointLat { get; set; }

    public double StartPointLng { get; set; }

    public DateTime TimeStamp { get; set; }
    
    public string AddressName { get; set; }

    public double Cost { get; set; }

    public UserIdentifier Driver { get; set; }

    public UserIdentifier User { get; set; }

    public InternRideStatus Status { get; set; }
    
    public CompleteStatus CompleteStatus { get; set; }
}