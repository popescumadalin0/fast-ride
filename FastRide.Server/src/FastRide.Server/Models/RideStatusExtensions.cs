using System;
using FastRide.Server.Contracts.Enums;
using Microsoft.DurableTask.Client;

namespace FastRide.Server.Models;

public static class RideStatusExtensions
{
    public static RideStatus ToRideStatus(this OrchestrationRuntimeStatus status)
    {
        return status switch
        {
            OrchestrationRuntimeStatus.Suspended or OrchestrationRuntimeStatus.Running
                or OrchestrationRuntimeStatus.Pending => RideStatus.InProgress,
            OrchestrationRuntimeStatus.Completed => RideStatus.Finished,
            OrchestrationRuntimeStatus.Terminated or OrchestrationRuntimeStatus.Failed => RideStatus.Cancelled,
            _ => RideStatus.None
        };
    }
}