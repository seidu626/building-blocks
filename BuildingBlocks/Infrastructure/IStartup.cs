// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.IStartup
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure;

public interface IStartup
{
  void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration);

  void Configure(IApplicationBuilder application);

  bool Enabled { get; }

  int Order { get; }
}