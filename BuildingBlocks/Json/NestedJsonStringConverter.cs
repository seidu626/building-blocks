namespace BuildingBlocks.Json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class NestedJsonStringConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string token, but got {reader.TokenType}");

        string nestedJson = reader.GetString();
        if (string.IsNullOrEmpty(nestedJson))
            return default(T);

        return JsonSerializer.Deserialize<T>(nestedJson, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        string nestedJson = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(nestedJson);
    }
}
