// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.ScheduleTasks.ScheduleTask
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.SeedWork;

namespace BuildingBlocks.ScheduleTasks;

public class ScheduleTask : Entity
{
  public string Name { get; set; }

  public int Seconds { get; set; }

  public string Type { get; set; }

  public bool Enabled { get; set; }

  public bool StopOnError { get; set; }

  public DateTime? LastStartUtc { get; set; }

  public DateTime? LastEndUtc { get; set; }

  public DateTime? LastSuccessUtc { get; set; }
}