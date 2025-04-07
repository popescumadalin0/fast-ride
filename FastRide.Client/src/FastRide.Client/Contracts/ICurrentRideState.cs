using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Enums;

namespace FastRide.Client.Contracts;

public interface ICurrentRideState
{
    event Action OnChange;
    RideStatus State { get; }
    string InstanceId { get; set; }
    void ResetState();
    Task UpdateState();
}