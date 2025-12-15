using System.Text.Json.Serialization;

namespace OdooBackend.Models;

public class JsonRpcErrorData
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
