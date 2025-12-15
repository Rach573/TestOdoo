using System.Text.Json.Serialization;

namespace OdooBackend.Models;

public class OdooAuthResult
{
    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("db")]
    public string? Db { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
