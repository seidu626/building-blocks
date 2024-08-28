// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.ServiceActivator
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure;

public static class ServiceActivator
{
  private static IServiceProvider _serviceProvider;

  public static void Configure(IServiceProvider serviceProvider)
  {
    ServiceActivator._serviceProvider = serviceProvider;
  }

  public static IServiceScope GetScope(IServiceProvider serviceProvider = null)
  {
    IServiceProvider serviceProvider1 = serviceProvider ?? ServiceActivator._serviceProvider;
    return serviceProvider1 == null ? (IServiceScope) null : ServiceProviderServiceExtensions.GetRequiredService<IServiceScopeFactory>(serviceProvider1).CreateScope();
  }

  public static IServiceProvider GetServiceProvider(IServiceScope scope = null)
  {
    if (scope != null)
      return scope.ServiceProvider;
    IServiceProvider serviceProvider = ServiceActivator._serviceProvider;
    return (serviceProvider != null ? ServiceProviderServiceExtensions.GetService<IHttpContextAccessor>(serviceProvider) : (IHttpContextAccessor) null)?.HttpContext?.RequestServices ?? ServiceActivator._serviceProvider;
  }
}