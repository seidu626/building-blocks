// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.IDataProvider
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using System.Data.Common;

#nullable disable
namespace BuildingBlocks.Persistence
{
  public interface IDataProvider
  {
    bool StoredProceduresSupported { get; }

    DbParameter GetParameter();

    string ProviderInvariantName { get; }
  }
}
