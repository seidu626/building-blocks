// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.Int32Extensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Diagnostics;

namespace BuildingBlocks.Extensions;

public static class Int32Extensions
{
  [DebuggerStepThrough]
  public static 
#nullable disable
    IEnumerable<int> Times(this int times)
  {
    for (int i = 1; i <= times; ++i)
      yield return i;
  }

  [DebuggerStepThrough]
  public static void Times(this int times, Action<int> actionFn)
  {
    for (int index = 1; index <= times; ++index)
      actionFn(index);
  }

  [DebuggerStepThrough]
  public static void Times(this int times, Action actionFn)
  {
    for (int index = 1; index <= times; ++index)
      actionFn();
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> Times<T>(this int times, Func<T> actionFn)
  {
    List<T> objList = new List<T>();
    for (int index = 1; index <= times; ++index)
      objList.Add(actionFn());
    return (IReadOnlyList<T>) objList;
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> Times<T>(this int times, Func<int, T> actionFn)
  {
    List<T> objList = new List<T>();
    for (int index = 1; index <= times; ++index)
      objList.Add(actionFn(index));
    return (IReadOnlyList<T>) objList;
  }

  [DebuggerStepThrough]
  public static TimeSpan Ticks(this int number) => TimeSpan.FromTicks((long) number);

  [DebuggerStepThrough]
  public static TimeSpan Milliseconds(this int number)
  {
    return TimeSpan.FromMilliseconds((double) number);
  }

  [DebuggerStepThrough]
  public static TimeSpan Seconds(this int number) => TimeSpan.FromSeconds((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Minutes(this int number) => TimeSpan.FromMinutes((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Hours(this int number) => TimeSpan.FromHours((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Days(this int number) => TimeSpan.FromDays((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Weeks(this int number) => TimeSpan.FromDays((double) (number * 7));
}