using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FastRide.Client.Authentication;

public class CustomUserAccount : RemoteUserAccount
{
    [JsonPropertyName("Roles")] public string[] Roles { get; set; }
}