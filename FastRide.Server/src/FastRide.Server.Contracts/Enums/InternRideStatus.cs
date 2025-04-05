namespace FastRide.Server.Contracts.Enums;

public enum InternRideStatus
{
    None = 0,
    NewRideAvailable = 1,
    GoingToUser = 3,
    GoingToDestination = 5,
    Finished = 7,
    Cancelled = 8,
}