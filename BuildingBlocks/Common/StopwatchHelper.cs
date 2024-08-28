// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.StopwatchHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;

namespace BuildingBlocks.Common;

public static class StopwatchHelper
{
  public static double GetDurationInMillisecondsSince(long startTime)
  {
    return StopwatchHelper.GetDurationInMilliseconds(startTime, Stopwatch.GetTimestamp());
  }

  public static double GetDurationInSecondsSince(long startTime)
  {
    return StopwatchHelper.GetDurationInSeconds(startTime, Stopwatch.GetTimestamp());
  }

  public static TimeSpan GetDurationSince(long startTime)
  {
    return StopwatchHelper.GetDuration(startTime, Stopwatch.GetTimestamp());
  }

  public static TimeSpan GetDuration(long from, long to)
  {
    return TimeSpan.FromMilliseconds(StopwatchHelper.GetDurationInMilliseconds(from, to));
  }

  public static double GetDurationInMilliseconds(long from, long to)
  {
    return StopwatchHelper.GetDurationInSeconds(from, to) * 1000.0;
  }

  public static double GetDurationInSeconds(long from, long to)
  {
    return ((double) to - (double) from) / (double) Stopwatch.Frequency;
  }
}