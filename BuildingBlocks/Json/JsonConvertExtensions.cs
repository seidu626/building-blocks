// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Json.JsonConvertExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Json;

public static class JsonConvertExtensions
{
  public static T TryDeserializeObject<T>(
    this string json,
    ILogger logger,
    JsonSerializerSettings options = null)
  {
    try
    {
      return JsonConvert.DeserializeObject<T>(json, options);
    }
    catch (JsonReaderException ex)
    {
      if (logger != null)
        logger.LogError((Exception) ex, "JSON deserialization failed for type {TypeName}.", (object) typeof (T).Name);
      return default (T);
    }
    catch (JsonSerializationException ex)
    {
      if (logger != null)
        logger.LogError((Exception) ex, "JSON deserialization failed for type {TypeName}.", (object) typeof (T).Name);
      return default (T);
    }
  }
}