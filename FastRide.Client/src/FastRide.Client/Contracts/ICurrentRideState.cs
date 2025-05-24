using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;

namespace FastRide.Client.Contracts;

public interface ICurrentRideState
{
    event Func<Task> OnChange;
    
    RideStatus State { get; }
    
    string InstanceId { get; set; }
    
    Task UpdateState(Ride ride);
}