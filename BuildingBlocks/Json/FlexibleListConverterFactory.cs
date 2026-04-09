namespace BuildingBlocks.Json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FlexibleListConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Check if typeToConvert is List<T>
        if (!typeToConvert.IsGenericType)
            return false;

        if (typeToConvert.GetGenericTypeDefinition() != typeof(List<>))
            return false;

        return true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Get the generic type T
        Type itemType = typeToConvert.GetGenericArguments()[0];

        // Create the converter type FlexibleListConverter<T>
        Type converterType = typeof(FlexibleListConverter<>).MakeGenericType(itemType);

        // Instantiate the converter
        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}
