using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Json;

public class FlexibleLongConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (long.TryParse(reader.GetString(), out var value))
                return value;

            throw new JsonException($"Cannot convert {reader.GetString()} to long.");
        }

        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetInt64();

        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing long.");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}