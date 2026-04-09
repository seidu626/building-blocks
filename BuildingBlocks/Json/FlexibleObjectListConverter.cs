namespace BuildingBlocks.Json;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FlexibleObjectListConverter<T> : JsonConverter<List<T>>
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<T>();

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // Deserialize the array
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                // Deserialize each object in the array
                T item = JsonSerializer.Deserialize<T>(ref reader, options);
                list.Add(item);
            }
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            // Deserialize a single object
            T item = JsonSerializer.Deserialize<T>(ref reader, options);
            list.Add(item);
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else
        {
            throw new JsonException($"Unexpected token type {reader.TokenType} when parsing a list of objects.");
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        if (value.Count == 1)
        {
            // Serialize as single object
            JsonSerializer.Serialize(writer, value[0], options);
        }
        else
        {
            // Serialize as array of objects
            writer.WriteStartArray();
            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }
            writer.WriteEndArray();
        }
    }
}
