using System.Text.Json.Serialization;

namespace OdooBackend.Models;

public class JsonRpcError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public JsonRpcErrorData? Data { get; set; }
}
