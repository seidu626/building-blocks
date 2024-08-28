// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.ClaimConverter
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuildingBlocks.Ldap;

public class ClaimConverter : JsonConverter
{
  public override bool CanConvert(Type objectType) => typeof (Claim) == objectType;

  public override object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer)
  {
      JObject jobject = JObject.Load(reader);
      return (object) new Claim(jobject.GetValue("Type").ToString(), jobject.GetValue("Value").ToString(), jobject.GetValue("ValueType").ToString());
    }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
      Claim claim = (Claim) value;
      new JObject()
      {
        {
          "Type",
          (JToken) claim.Type
        },
        {
          "Value",
          (JToken) claim.Value
        },
        {
          "ValueType",
          (JToken) claim.ValueType
        }
      }.WriteTo(writer);
    }
}