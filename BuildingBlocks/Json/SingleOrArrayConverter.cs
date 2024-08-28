using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Json;

public class SingleOrArrayConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Checking if the current token is the start of an array.
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read(); // Move to the first element of the array or the end array token.
            string result = reader.GetString(); // Read the string value.
            reader.Read(); // Move past the end array token.
            return result; // Return the string read from the array.
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString(); // Return the string directly.
        }
        else
        {
            // Throw an exception if the token is neither a string nor the start of an array.
            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // Writing the string value directly as a JSON string.
        writer.WriteStringValue(value);
    }
}