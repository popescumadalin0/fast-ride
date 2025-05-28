using FastRide.Server.Contracts.Enums;

namespace FastRide.Server.Contracts.Models;

public class User
{
    public UserIdentifier Identifier { get; set; }
    public UserType UserType { get; set; }

    public double Rating { get; set; }

    public string PictureUrl { get; set; }

    public string PhoneNumber { get; set; }
    
    public string UserName { get; set; }
}