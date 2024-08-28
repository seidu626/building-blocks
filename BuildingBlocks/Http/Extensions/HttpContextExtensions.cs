// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Http.Extensions.HttpContextExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Http.Extensions;

public static class HttpContextExtensions
{
  public static string GetMetricsCurrentResourceName(this HttpContext httpContext)
  {
    if (httpContext == null)
      throw new ArgumentNullException(nameof (httpContext));
    return httpContext.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
  }
}