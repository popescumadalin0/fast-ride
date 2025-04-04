using System.Collections.Generic;
using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Models;

public class DriverAcceptResponse
{
    public string UserId { get; set; }

    public bool Accepted { get; set; }
}