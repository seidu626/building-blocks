// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.Int16Extensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Diagnostics;

namespace BuildingBlocks.Extensions;

public static class Int16Extensions
{
  [DebuggerStepThrough]
  public static 
#nullable disable
    IEnumerable<short> Times(this short times)
  {
    for (short i = 1; (int) i <= (int) times; ++i)
      yield return i;
  }

  [DebuggerStepThrough]
  public static void Times(this short times, Action<short> actionFn)
  {
    for (short index = 1; (int) index <= (int) times; ++index)
      actionFn(index);
  }

  [DebuggerStepThrough]
  public static void Times(this short times, Action actionFn)
  {
    for (short index = 1; (int) index <= (int) times; ++index)
      actionFn();
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> Times<T>(this short times, Func<T> actionFn)
  {
    List<T> objList = new List<T>();
    for (short index = 1; (int) index <= (int) times; ++index)
      objList.Add(actionFn());
    return (IReadOnlyList<T>) objList;
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> Times<T>(this short times, Func<short, T> actionFn)
  {
    List<T> objList = new List<T>();
    for (short index = 1; (int) index <= (int) times; ++index)
      objList.Add(actionFn(index));
    return (IReadOnlyList<T>) objList;
  }

  [DebuggerStepThrough]
  public static TimeSpan Ticks(this short number) => TimeSpan.FromTicks((long) number);

  [DebuggerStepThrough]
  public static TimeSpan Milliseconds(this short number)
  {
    return TimeSpan.FromMilliseconds((double) number);
  }

  [DebuggerStepThrough]
  public static TimeSpan Seconds(this short number) => TimeSpan.FromSeconds((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Minutes(this short number) => TimeSpan.FromMinutes((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Hours(this short number) => TimeSpan.FromHours((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Days(this short number) => TimeSpan.FromDays((double) number);

  [DebuggerStepThrough]
  public static TimeSpan Weeks(this short number)
  {
    return TimeSpan.FromDays((double) ((int) number * 7));
  }
}