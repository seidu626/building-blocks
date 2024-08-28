// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Options.SwaggerOptions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Options;

public class SwaggerOptions
{
  public bool IsEnabled { get; set; }

  public string JsonRoute { get; set; }

  public string Description { get; set; }

  public string UiEndpoint { get; set; }

  public string Version { get; set; }

  public SwaggerContact Contact { get; set; } = new SwaggerContact();

  public SwaggerLicense License { get; set; } = new SwaggerLicense();
}