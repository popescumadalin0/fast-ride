using System.ComponentModel;

namespace FastRide.Client.Enums;

public enum RideState
{
    None = 0,
    [Description("Finding a driver")] FindingDriver = 2,

    [Description("Driver found! Wait for they...")]
    GoingToUser = 3,

    [Description("Driver arrived! Going to destination...")]
    GoingToDestination = 4,
    [Description("Arrived!")] Finished = 5,
    [Description("Ride cancelled!")] Cancelled = 6,
}