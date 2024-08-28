// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Clock
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using BuildingBlocks.Common.Interfaces;

namespace BuildingBlocks.Common;

public sealed class Clock : IClock
{
  public static Clock Instance { get; } = new Clock();

  [DebuggerStepThrough]
  private Clock()
  {
    try
    {
      NativeMethods.GetSystemTimePreciseAsFileTime(out long _);
      this.IsPrecise = true;
    }
    catch (Exception ex) when (ex is EntryPointNotFoundException || ex is DllNotFoundException)
    {
      this.IsPrecise = false;
    }
  }

  public bool IsPrecise { get; }

  public DateTimeOffset Now
  {
    get
    {
      if (!this.IsPrecise)
        return DateTimeOffset.Now;
      long filetime;
      NativeMethods.GetSystemTimePreciseAsFileTime(out filetime);
      return DateTimeOffset.FromFileTime(filetime);
    }
  }
}