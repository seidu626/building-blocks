﻿// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.ITrackable
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks;

public interface ITrackable
{
  DateTime CreatedAt { get; set; }

  string Username { get; set; }

  DateTime LastUpdatedAt { get; set; }

  string LastUpdatedBy { get; set; }
}