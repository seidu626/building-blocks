// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.DateTimeSettings
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Configuration;

namespace BuildingBlocks.Helpers;

public class DateTimeSettings : ISettings
{
  public string DefaultTimeZoneId { get; set; }

  public bool AllowCustomersToSetTimeZone { get; set; }
}