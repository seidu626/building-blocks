// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Json.JsonSerializerExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Ldap.Json;

public static class JsonSerializerExtensions
{
  public static T TryDeserialize<T>(
    this string json,
    ILogger logger,
    JsonSerializerOptions options = null)
  {
    try
    {
      return JsonSerializer.Deserialize<T>(json, options);
    }
    catch (JsonException ex)
    {
      if (logger != null)
        logger.LogError((Exception) ex, "JSON deserialization failed for type {TypeName} with payload: {Payload}.", (object) typeof (T).Name, (object) json);
      return default (T);
    }
  }
}