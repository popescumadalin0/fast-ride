using System.ComponentModel;

namespace FastRide.Server.Contracts.Enums;

public enum RideStatus
{
    //for driver
    [Description("New ride available!")] NewRideAvailable = 1, 
    [Description("Ride in progress! Go to user!")]
    DriverGoingToUser = 3, 
    [Description("Ride in progress! Wait user and go to destination!")]
    DriverGoingToDestination = 5,
    //for user
    [Description("Finding a driver")] FindingDriver = 2, 
    [Description("Driver found! Wait for they...")]
    GoingToUser = 4,
    [Description("Driver arrived! Going to destination...")]
    GoingToDestination = 6,
    
    //common
    None = 0,
    [Description("Arrived!")] Finished = 7,
    [Description("Ride cancelled!")] Cancelled = 8,
}