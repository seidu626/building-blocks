// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Configuration.DistributedCacheConfig
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildingBlocks.Configuration;

public class DistributedCacheConfig : IConfig
{
  [JsonConverter(typeof (StringEnumConverter))]
  public DistributedCacheType DistributedCacheType { get; set; } = DistributedCacheType.Redis;

  public bool Enabled { get; set; }

  public string ConnectionString { get; set; } = "127.0.0.1:6379,ssl=False";

  public string SchemaName { get; set; } = "dbo";

  public string TableName { get; set; } = "DistributedCache";
}