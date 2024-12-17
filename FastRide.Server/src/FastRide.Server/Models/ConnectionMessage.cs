using Newtonsoft.Json;

namespace FastRide.Server.Models;

[JsonObject]
public abstract class ConnectionMessage
{
    [JsonProperty("connectionId")]
    public string ConnectionId { get; set; }
}