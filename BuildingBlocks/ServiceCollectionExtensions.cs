// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.ServiceCollectionExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Pipeline;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
  {
    return ServiceCollectionServiceExtensions.AddSingleton(ServiceCollectionServiceExtensions.AddSingleton(services, typeof (IPipelineBehavior<,>), typeof (ErrorLoggingBehaviour<,>)), typeof (IPipelineBehavior<,>), typeof (MessageValidatorBehaviour<,>));
  }
}