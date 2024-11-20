using System;

namespace FastRide.Server.Authentication;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    public string[] UserRoles { get; set; } = Array.Empty<string>();
}