// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.OAuth.TokenHandler
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.OAuth;

public class TokenHandler : DelegatingHandler
{
  private readonly 
#nullable disable
    IHttpContextAccessor _httpContextAccessor;

  public TokenHandler(IHttpContextAccessor httpContextAccessor)
  {
    this._httpContextAccessor = httpContextAccessor;
  }

  protected virtual async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await AuthenticationHttpContextExtensions.GetTokenAsync(this._httpContextAccessor.HttpContext, "access_token"));
    return await base.SendAsync(request, cancellationToken);
  }
}