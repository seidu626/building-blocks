// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.ExceptionExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace BuildingBlocks.Extensions;

public static class ExceptionExtensions
{
  public static bool IsFatal(this 
#nullable disable
    Exception ex)
  {
    switch (ex)
    {
      case StackOverflowException _:
      case OutOfMemoryException _:
      case AccessViolationException _:
      case AppDomainUnloadedException _:
      case ThreadAbortException _:
      case SecurityException _:
        return true;
      default:
        return ex is SEHException;
    }
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx>(this Exception ex) where TEx : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e => e is TEx));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx1, TEx2>(this Exception ex)
    where TEx1 : Exception
    where TEx2 : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e => e is TEx1 || e is TEx2));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx1, TEx2, TEx3>(this Exception ex)
    where TEx1 : Exception
    where TEx2 : Exception
    where TEx3 : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e =>
    {
      switch (e)
      {
        case TEx1 _:
        case TEx2 _:
          return true;
        default:
          return e is TEx3;
      }
    }));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4>(this Exception ex)
    where TEx1 : Exception
    where TEx2 : Exception
    where TEx3 : Exception
    where TEx4 : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e =>
    {
      switch (e)
      {
        case TEx1 _:
        case TEx2 _:
        case TEx3 _:
          return true;
        default:
          return e is TEx4;
      }
    }));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5>(this Exception ex)
    where TEx1 : Exception
    where TEx2 : Exception
    where TEx3 : Exception
    where TEx4 : Exception
    where TEx5 : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e =>
    {
      switch (e)
      {
        case TEx1 _:
        case TEx2 _:
        case TEx3 _:
        case TEx4 _:
          return true;
        default:
          return e is TEx5;
      }
    }));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6>(this Exception ex)
    where TEx1 : Exception
    where TEx2 : Exception
    where TEx3 : Exception
    where TEx4 : Exception
    where TEx5 : Exception
    where TEx6 : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e =>
    {
      switch (e)
      {
        case TEx1 _:
        case TEx2 _:
        case TEx3 _:
        case TEx4 _:
        case TEx5 _:
          return true;
        default:
          return e is TEx6;
      }
    }));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7>(
    this Exception ex)
    where TEx1 : Exception
    where TEx2 : Exception
    where TEx3 : Exception
    where TEx4 : Exception
    where TEx5 : Exception
    where TEx6 : Exception
    where TEx7 : Exception
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e =>
    {
      switch (e)
      {
        case TEx1 _:
        case TEx2 _:
        case TEx3 _:
        case TEx4 _:
        case TEx5 _:
        case TEx6 _:
          return true;
        default:
          return e is TEx7;
      }
    }));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException(this Exception ex, params Type[] exceptionTypes)
  {
    return ex.IsExpectedException((Func<Exception, bool>) (e => ((IEnumerable<Type>) exceptionTypes).Contains<Type>(e.GetType())));
  }

  [DebuggerStepThrough]
  public static bool IsExpectedException(this Exception ex, Func<Exception, bool> predicate)
  {
    if (predicate(ex))
      return true;
    if (!(ex is AggregateException aggregateException))
      return false;
    bool found = false;
    aggregateException.Flatten().Handle((Func<Exception, bool>) (x =>
    {
      if (predicate(x))
        found = true;
      return true;
    }));
    return found;
  }
}