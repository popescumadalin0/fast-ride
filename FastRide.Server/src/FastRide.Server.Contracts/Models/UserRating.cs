namespace FastRide.Server.Contracts.Models;

public class UserRating
{
    public UserIdentifier User { get; set; }
    public int Rating { get; set; }
}