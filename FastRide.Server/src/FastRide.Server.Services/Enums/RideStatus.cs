namespace FastRide.Server.Services.Entities;

public enum RideStatus
{
    None = 0,
    Draft = 1,
    Pending = 2,
    InProgress = 3,
    Finished = 4,
    Cancelled = 5,
}