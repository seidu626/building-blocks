// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.Mapper.AutoMapperConfiguration
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using AutoMapper;

namespace BuildingBlocks.Infrastructure.Mapper;

public static class AutoMapperConfiguration
{
  public static IMapper Mapper { get; private set; }

  public static MapperConfiguration MapperConfiguration { get; private set; }

  public static void Init(MapperConfiguration config)
  {
    AutoMapperConfiguration.MapperConfiguration = config;
    AutoMapperConfiguration.Mapper = config.CreateMapper();
  }
}