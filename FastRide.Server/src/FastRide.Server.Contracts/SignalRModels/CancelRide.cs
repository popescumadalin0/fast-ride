using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Contracts.SignalRModels;

public class CancelRide
{
    public string InstanceId { get; set; }
}