namespace FastRide.Server.Contracts;

public class UserRating
{
    public UserIdentifier User { get; set; }
    public int Rating { get; set; }
}