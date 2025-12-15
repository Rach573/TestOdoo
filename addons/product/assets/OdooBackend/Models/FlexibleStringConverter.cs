using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OdooBackend.Models;

/// <summary>
/// Allows deserializing values that can be string/number/bool/null into a string.
/// Odoo may return non-string default_code values, so we coerce to string instead of failing.
/// </summary>
public class FlexibleStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.TryGetInt64(out var l) ? l.ToString() : reader.GetDouble().ToString(),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Null => null,
            _ => JsonDocument.ParseValue(ref reader).RootElement.ToString()
        };
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
