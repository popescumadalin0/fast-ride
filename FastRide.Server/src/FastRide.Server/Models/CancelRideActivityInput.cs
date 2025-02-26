using System.Collections.Generic;
using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Models;

public class CancelRideActivityInput
{
    public string InstanceId { get; set; }

    public string UserId { get; set; }
}