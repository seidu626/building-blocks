// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.DbQuerySettings
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

#nullable disable
namespace BuildingBlocks.Persistence
{
  public class DbQuerySettings
  {
    public DbQuerySettings(bool ignoreAcl, bool ignoreMultiStore)
    {
      this.IgnoreAcl = ignoreAcl;
      this.IgnoreMultiStore = ignoreMultiStore;
    }

    public bool IgnoreAcl { get; private set; }

    public bool IgnoreMultiStore { get; private set; }

    public static DbQuerySettings Default { get; } = new DbQuerySettings(false, false);
  }
}
