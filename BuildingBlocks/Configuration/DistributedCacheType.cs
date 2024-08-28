// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Configuration.DistributedCacheType
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.Serialization;

namespace BuildingBlocks.Configuration;

public enum DistributedCacheType
{
  [EnumMember(Value = "memory")] Memory,
  [EnumMember(Value = "sqlserver")] SqlServer,
  [EnumMember(Value = "redis")] Redis,
}