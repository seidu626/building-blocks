// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Configuration.CommonConfig
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Configuration;

public class CommonConfig : IConfig
{
  public bool RequestLogging { get; set; }

  public bool EnableSwagger { get; set; }

  public bool DisplayFullErrorStack { get; set; }

  public string UserAgentStringsPath { get; set; } = "~/App_Data/browscap.xml";

  public string CrawlerOnlyUserAgentStringsPath { get; set; } = "~/App_Data/browscap.crawlersonly.xml";

  public bool UseSessionStateTempDataProvider { get; set; }

  public bool MiniProfilerEnabled { get; set; }

  public int? ScheduleTaskRunTimeout { get; set; }

  public bool UseHsts { get; set; }

  public bool UseHttpsRedirection { get; set; }

  public int HttpsRedirectionRedirect { get; set; } = 308;

  public int HttpsRedirectionHttpsPort { get; set; } = 443;

  public bool UseUrlRewrite { get; set; }

  public bool UrlRewriteHttpsOptions { get; set; }

  public int UrlRewriteHttpsOptionsStatusCode { get; set; }

  public int UrlRewriteHttpsOptionsPort { get; set; }

  public bool UrlRedirectToHttpsPermanent { get; set; }

  public string[] CorsWebOrigins { get; set; }

  public string StaticFilesCacheControl { get; set; } = "public,max-age=31536000";
}