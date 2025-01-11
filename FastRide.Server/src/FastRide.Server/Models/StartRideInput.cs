using FastRide.Server.Contracts.Models;
using Newtonsoft.Json;

namespace FastRide.Server.Models;

public class StartRideInput
{
    public StartRide Ride { get; set; }
}