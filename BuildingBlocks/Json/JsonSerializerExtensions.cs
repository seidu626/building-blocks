using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Json
{
    public static class JsonSerializerExtensions
    {
        public static T? TryDeserialize<T>(
            this string json,
            ILogger logger,
            JsonSerializerOptions? options = null)
        {
            try
            {
                // If no options provided, create default with the converter factory
                if (options == null)
                {
                    options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters =
                        {
                            new FlexibleListConverterFactory(),
                            new FlexibleLongConverterFactory(),
                            new FlexibleObjectListConverterFactory(),
                            new FlexibleObjectListConverterFactory()
                            // Add other converter factories or custom converters here if needed
                        }
                    };
                }
                else
                {
                    // Ensure the converter factory is included
                    bool hasFactory = false;
                    foreach (var converter in options.Converters)
                    {
                        if (converter is FlexibleListConverterFactory)
                        {
                            hasFactory = true;
                            break;
                        }
                    }

                    if (!hasFactory)
                    {
                        options.Converters.Add(new FlexibleListConverterFactory());
                    }
                }

                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (JsonException ex)
            {
                if (logger != null)
                    logger.LogError(ex,
                        "JSON deserialization failed for type {TypeName} with payload: {Payload}.",
                        typeof(T).Name,
                        json);
                return default(T);
            }
        }
    }
}