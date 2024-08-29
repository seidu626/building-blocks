// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Extensions.SqlServerInfo
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

#nullable disable
namespace BuildingBlocks.Persistence.Extensions
{
  public class SqlServerInfo
  {
    public string ProductVersion { get; set; }

    public string PatchLevel { get; set; }

    public string ProductEdition { get; set; }

    public string ClrVersion { get; set; }

    public string DefaultCollation { get; set; }

    public string Instance { get; set; }

    public int Lcid { get; set; }

    public string ServerName { get; set; }
  }
}
