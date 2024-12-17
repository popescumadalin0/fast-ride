using Newtonsoft.Json;

namespace FastRide.Server.Models;

[JsonObject]
public abstract class GroupMessage
{
    [JsonProperty("groupName")]
    public string GroupName { get; set; }
}