using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Json;

public class NestedJsonStringConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Check if typeToConvert is T (for any T)
        // Alternatively, restrict to specific types if needed
        return true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Create the converter type NestedJsonStringConverter<T>
        Type converterType = typeof(NestedJsonStringConverter<>).MakeGenericType(typeToConvert);

        // Instantiate the converter
        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}
