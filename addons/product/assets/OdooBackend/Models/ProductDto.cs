using System.Text.Json.Serialization;

namespace OdooBackend.Models;

public class ProductDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("list_price")]
    public decimal? ListPrice { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("default_code")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string? DefaultCode { get; set; }

    [JsonPropertyName("categ_id")]
    public object? CategId { get; set; }

    [JsonPropertyName("qty_available")]
    public decimal? QtyAvailable { get; set; }
}
