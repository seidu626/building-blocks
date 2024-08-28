// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Logging.LoggerExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging;

public static class LoggerExtensions
{
  public static IDisposable BeginNamedScope(
    this ILogger logger,
    string name,
    params (string, object)[] properties)
  {
    Dictionary<string, object> dictionary = ((IEnumerable<(string, object)>) properties).ToDictionary<(string, object), string, object>((Func<(string, object), string>) (p => p.Item1), (Func<(string, object), object>) (p => p.Item2));
    dictionary[name + ".Scope"] = (object) Guid.NewGuid();
    return logger.BeginScope<Dictionary<string, object>>(dictionary);
  }

  public static IDisposable BeginPropertyScope(
    this ILogger logger,
    params (string, object)[] properties)
  {
    Dictionary<string, object> dictionary = ((IEnumerable<(string, object)>) properties).ToDictionary<(string, object), string, object>((Func<(string, object), string>) (p => p.Item1), (Func<(string, object), object>) (p => p.Item2));
    return logger.BeginScope<Dictionary<string, object>>(dictionary);
  }
}