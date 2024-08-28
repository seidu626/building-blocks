// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.FakeClock
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Common.Interfaces;

namespace BuildingBlocks.Common;

public sealed class FakeClock : IClock
{
  public FakeClock(DateTimeOffset offset, bool isPrecise = true)
  {
    this.Now = offset;
    this.IsPrecise = isPrecise;
  }

  public bool IsPrecise { get; }

  public DateTimeOffset Now { get; }
}