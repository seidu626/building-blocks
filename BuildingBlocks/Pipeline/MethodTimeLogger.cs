using System.Reflection;
using ILogger = Serilog.ILogger;

namespace BuildingBlocks.Pipeline;

public static class MethodTimeLogger
{
  public static ILogger Logger;

  public static void Log(MethodBase methodBase, TimeSpan timeSpan, string message)
  {
    MethodTimeLogger.Logger.Information("MethodTimer: {Class}.{Method} - {Message} in {Duration}", (object) ((MemberInfo) methodBase).DeclaringType.FullName, (object) ((MemberInfo) methodBase).Name, (object) message, (object) timeSpan);
  }
}   