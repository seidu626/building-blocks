// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Pipeline.MethodTimeLogger
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Pipeline;

public static class MethodTimeLogger
{
  public static ILogger Logger;

  public static void Log(MethodBase methodBase, TimeSpan timeSpan, string message)
  {
    MethodTimeLogger.Logger.LogTrace("{Class}.{Method} - {Message} in {Duration}", (object) ((MemberInfo) methodBase).DeclaringType.FullName, (object) ((MemberInfo) methodBase).Name, (object) message, (object) timeSpan);
  }
}