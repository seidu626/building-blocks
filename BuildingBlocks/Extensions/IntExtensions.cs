// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.Int64Extensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Diagnostics;

namespace BuildingBlocks.Extensions;

public static class Int64Extensions
{
  [DebuggerStepThrough]
  public static DateTime FromEpochMilliseconds(this long epochMilliseconds)
  {
    return DateTimeExtensions.Epoch.AddMilliseconds((double) epochMilliseconds);
  }

  [DebuggerStepThrough]
  public static 
#nullable disable
    IEnumerable<long> Times(this long times)
  {
    for (long i = 1; i <= times; ++i)
      yield return i;
  }

  [DebuggerStepThrough]
  public static void Times(this long times, Action<long> actionFn)
  {
    for (long index = 1; index <= times; ++index)
      actionFn(index);
  }

  [DebuggerStepThrough]
  public static void Times(this long times, Action actionFn)
  {
    for (long index = 1; index <= times; ++index)
      actionFn();
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> Times<T>(this long times, Func<T> actionFn)
  {
    List<T> objList = new List<T>();
    for (long index = 1; index <= times; ++index)
      objList.Add(actionFn());
    return (IReadOnlyList<T>) objList;
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> Times<T>(this long times, Func<long, T> actionFn)
  {
    List<T> objList = new List<T>();
    for (long index = 1; index <= times; ++index)
      objList.Add(actionFn(index));
    return (IReadOnlyList<T>) objList;
  }

  [DebuggerStepThrough]
  public static TimeSpan Ticks(this long number) => TimeSpan.FromTicks(number);

  [DebuggerStepThrough]
  public static TimeSpan Milliseconds(this long number)
  {
    return TimeSpan.FromMilliseconds((double) number);
  }

  [DebuggerStepThrough]
  public static TimeSpan Seconds(this long number) => TimeSpan.FromSeconds((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Minutes(this long number) => TimeSpan.FromMinutes((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Hours(this long number) => TimeSpan.FromHours((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Days(this long number) => TimeSpan.FromDays((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Weeks(this long number) => TimeSpan.FromDays((double) (number * 7L));
}