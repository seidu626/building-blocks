// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Http.Extensions.SessionExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BuildingBlocks.Http.Extensions;

public static class SessionExtensions
{
  public static void Set<T>(this ISession session, string key, T value)
  {
    session.SetString(key, JsonConvert.SerializeObject(value));
  }

  public static T Get<T>(this ISession session, string key)
  {
    string str = session.GetString(key);
    return str == null ? default (T) : JsonConvert.DeserializeObject<T>(str);
  }
}