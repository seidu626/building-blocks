namespace BuildingBlocks.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FlexibleListConverter<T> : JsonConverter<List<T>>
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

                T item = JsonSerializer.Deserialize<T>(ref reader, options);
                list.Add(item);
            }
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else
        {
            // Deserialize a single item
            T item = JsonSerializer.Deserialize<T>(ref reader, options);
            list.Add(item);
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
            // Serialize as single item
            JsonSerializer.Serialize(writer, value[0], options);
        }
        else
        {
            // Serialize as array
            writer.WriteStartArray();
            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }
            writer.WriteEndArray();
        }
    }
}

