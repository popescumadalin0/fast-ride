using System;
using FastRide.Server.Services.Enums;

namespace FastRide.Server.Authentication;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    public UserType[] UserRoles { get; set; } = [];
}