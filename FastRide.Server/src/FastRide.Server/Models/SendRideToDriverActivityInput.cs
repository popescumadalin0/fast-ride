using FastRide.Server.Contracts.Models;

namespace FastRide.Server.Models;

public class SendRideToDriverActivityInput
{
    public  FindDriverActivityInput RequestInput  { get; set; }
    public OnlineDriver Driver  { get; set; }
    
    public UserIdentifier User  { get; set; }
}