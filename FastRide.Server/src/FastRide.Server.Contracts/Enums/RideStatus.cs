using System.ComponentModel;

namespace FastRide.Server.Contracts.Enums;

public enum RideStatus
{
    //for driver
    [Description("New ride available")] NewRideAvailable = 1,

    [Description("Going to user")]
    DriverGoingToUser = 3,

    [Description("Going to destination")]
    DriverGoingToDestination = 5,

    //for user
    [Description("Finding a driver...")] FindingDriver = 2,

    [Description("Driver on the way...")]
    GoingToUser = 4,

    [Description("Going to destination")]
    GoingToDestination = 6,

    //common
    None = 0,
    [Description("Arrived")] Finished = 7,
    [Description("Ride cancelled")] Cancelled = 8,
}