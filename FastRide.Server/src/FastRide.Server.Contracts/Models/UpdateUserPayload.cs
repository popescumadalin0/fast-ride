using System.ComponentModel;
using FastRide.Server.Contracts.Enums;

namespace FastRide.Server.Contracts.Models;

public class UpdateUserPayload
{
    public UserIdentifier User { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public UserType? UserType { get; set; }
    
    public string PictureUrl { get; set; }
}