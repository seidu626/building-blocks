namespace BuildingBlocks.Json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FlexibleLongConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Check if typeToConvert is List<T>
        if (!typeToConvert.IsGenericType)
            return false;

        if (typeToConvert.GetGenericTypeDefinition() != typeof(long))
            return false;

        return true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Create the converter type FlexibleLongConverter
        Type converterType = typeof(FlexibleLongConverter).MakeGenericType(typeToConvert);

        // Instantiate the converter
        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}