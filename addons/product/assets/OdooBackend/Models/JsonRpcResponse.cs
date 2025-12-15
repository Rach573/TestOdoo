using System.Text.Json.Serialization;

namespace OdooBackend.Models;

public class JsonRpcResponse<TResult>
{
    [JsonPropertyName("result")]
    public TResult? Result { get; set; }

    [JsonPropertyName("error")]
    public JsonRpcError? Error { get; set; }
}
