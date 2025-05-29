using FastRide.Server.Contracts.Enums;

namespace FastRide.Server.Contracts.Models;

public class UpdateUserPayload
{
    public string PhoneNumber { get; set; }
    
    public UserType? UserType { get; set; }
}