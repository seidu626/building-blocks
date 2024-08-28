// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Configuration.CacheConfig
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Configuration;

public class CacheConfig : IConfig
{
  public int DefaultCacheTime { get; set; } = 60;

  public int ShortTermCacheTime { get; set; } = 3;

  public int BundledFilesCacheTime { get; set; } = 120;
}