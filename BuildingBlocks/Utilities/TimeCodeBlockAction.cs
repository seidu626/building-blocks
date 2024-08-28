// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Utilities.TimeCodeBlockAction
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;

namespace BuildingBlocks.Utilities;

public static class TimeCodeBlockAction
{
  public static TimeSpan TimeAction(this Action blockingAction)
  {
    Stopwatch stopwatch = Stopwatch.StartNew();
    blockingAction();
    stopwatch.Stop();
    return stopwatch.Elapsed;
  }
}