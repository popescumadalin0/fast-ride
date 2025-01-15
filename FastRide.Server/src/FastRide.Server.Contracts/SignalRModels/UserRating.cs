using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Contracts.SignalRModels;

public class UserRating
{
    public UserIdentifier User { get; set; }
    public int Rating { get; set; }
}