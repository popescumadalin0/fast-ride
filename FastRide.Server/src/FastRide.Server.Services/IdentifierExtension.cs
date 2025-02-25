using System;
using System.Security.Cryptography;
using System.Text;

namespace FastRide.Server.Services;

public static class IdentifierExtension
{
    public static Guid GenerateGuidFromString(this string input)
    {
        using SHA256 sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var guidBytes = new byte[16];
        Array.Copy(hashBytes, guidBytes, 16);
        return new Guid(guidBytes);
    }
}