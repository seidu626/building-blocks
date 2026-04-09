using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuildingBlocks.Ldap;

public class ClaimConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => typeof(Claim) == objectType;

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        var jobject = JObject.Load(reader);
        var type = jobject.GetValue("Type")?.ToString() ?? string.Empty;
        var value = jobject.GetValue("Value")?.ToString() ?? string.Empty;
        var valueType = jobject.GetValue("ValueType")?.ToString() ?? string.Empty;
        
        return new Claim(type, value, valueType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not Claim claim)
            return;
            
        var jobject = new JObject
        {
            ["Type"] = claim.Type,
            ["Value"] = claim.Value,
            ["ValueType"] = claim.ValueType
        };
        
        jobject.WriteTo(writer);
    }
}