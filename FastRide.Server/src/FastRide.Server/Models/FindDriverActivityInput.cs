using System.Collections.Generic;
using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Models;

public class FindDriverActivityInput
{
    public string InstanceId { get; set; }
    
    public Geolocation UserGeolocation { get; set; }

    public Geolocation Destination { get; set; }

    public List<string> ExcludeDrivers { get; set; }

    public string GroupName { get; set; }
}