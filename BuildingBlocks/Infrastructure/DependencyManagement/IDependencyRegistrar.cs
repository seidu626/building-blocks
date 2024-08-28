// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.DependencyManagement.IDependencyRegistrar
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.DependencyManagement;

public interface IDependencyRegistrar
{
  void Register(IServiceCollection services, ITypeFinder typeFinder);

  int Order { get; }
}