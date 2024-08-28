using System.Text.Json.Serialization;

namespace BuildingBlocks.OAuth;

// Define 'RoleInfo' as a record with properties 'Type' and 'Value'.
public record RoleInfo
{
    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("value")]
    public string Value { get; init; }

    // Override ToString() to provide a more detailed string representation if needed.
    public override string ToString()
    {
        return $"Type = {Type}, Value = {Value}";
    }
}