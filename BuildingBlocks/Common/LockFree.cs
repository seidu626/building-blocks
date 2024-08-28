// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.LockFree
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;

namespace BuildingBlocks.Common;

public static class LockFree
{
  [DebuggerStepThrough]
  public static void Update(ref object location, Func<object, object> generator)
  {
    SpinWait spinWait = new SpinWait();
    while (true)
    {
      object comparand = location;
      object obj1 = generator(comparand);
      object obj2 = Interlocked.CompareExchange(ref location, obj1, comparand);
      if (!comparand.Equals(obj2))
        spinWait.SpinOnce();
      else
        break;
    }
  }

  [DebuggerStepThrough]
  public static void Update<T>(ref T location, Func<T, T> generator) where T : class
  {
    SpinWait spinWait = new SpinWait();
    while (true)
    {
      T comparand = location;
      T obj1 = generator(comparand);
      T obj2 = Interlocked.CompareExchange<T>(ref location, obj1, comparand);
      if (!comparand.Equals((object) obj2))
        spinWait.SpinOnce();
      else
        break;
    }
  }

  [DebuggerStepThrough]
  public static void Update(ref int location, Func<int, int> generator)
  {
    SpinWait spinWait = new SpinWait();
    while (true)
    {
      int comparand = location;
      int num1 = generator(comparand);
      int num2 = Interlocked.CompareExchange(ref location, num1, comparand);
      if (!comparand.Equals(num2))
        spinWait.SpinOnce();
      else
        break;
    }
  }

  [DebuggerStepThrough]
  public static void Update(ref long location, Func<long, long> generator)
  {
    SpinWait spinWait = new SpinWait();
    while (true)
    {
      long comparand = location;
      long num1 = generator(comparand);
      long num2 = Interlocked.CompareExchange(ref location, num1, comparand);
      if (!comparand.Equals(num2))
        spinWait.SpinOnce();
      else
        break;
    }
  }

  [DebuggerStepThrough]
  public static void Update(ref float location, Func<float, float> generator)
  {
    SpinWait spinWait = new SpinWait();
    while (true)
    {
      float comparand = location;
      float num1 = generator(comparand);
      float num2 = Interlocked.CompareExchange(ref location, num1, comparand);
      if (!comparand.Equals(num2))
        spinWait.SpinOnce();
      else
        break;
    }
  }

  [DebuggerStepThrough]
  public static void Update(ref double location, Func<double, double> generator)
  {
    SpinWait spinWait = new SpinWait();
    while (true)
    {
      double comparand = location;
      double num1 = generator(comparand);
      double num2 = Interlocked.CompareExchange(ref location, num1, comparand);
      if (!comparand.Equals(num2))
        spinWait.SpinOnce();
      else
        break;
    }
  }
}