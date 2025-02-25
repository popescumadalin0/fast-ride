using FastRide.Server.Contracts.Enums;

namespace FastRide.Server.Contracts.Models;

public class OnlineDriver
{
    public UserIdentifier Identifier { get; set; }

    public Geolocation Geolocation { get; set; }

    public UserType UserType => UserType.Driver;

    public string GroupName { get; set; }
}