using Newtonsoft.Json;

namespace FastRide.Server.Models;

[JsonObject]
public class SignalRMessage
{
    [JsonProperty("connectionId")]
    public string ConnectionId { get; set; }

    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("groupName")]
    public string GroupName { get; set; }

    [JsonProperty("target"), JsonRequired]
    public string Target { get; set; }

    [JsonProperty("arguments"), JsonRequired]
    public object[] Arguments { get; set; }
}