// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.IWebHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks;

public interface IWebHelper
{
  string GetUrlReferrer();

  string GetCurrentIpAddress();

  string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

  bool IsCurrentConnectionSecured();

  string GetAppHost(bool useSsl);

  string GetAppLocation(bool? useSsl = null);

  bool IsStaticResource();

  string ModifyQueryString(string url, string key, params string[] values);

  string RemoveQueryString(string url, string key, string value = null);

  T QueryString<T>(string name);

  void RestartAppDomain();

  bool IsRequestBeingRedirected { get; }

  bool IsPostBeingDone { get; set; }

  string GetCurrentRequestProtocol();

  bool IsLocalRequest(HttpRequest req);

  string GetRawUrl(HttpRequest request);

  bool IsAjaxRequest(HttpRequest request);
}