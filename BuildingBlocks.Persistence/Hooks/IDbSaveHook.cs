// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Hooks.IDbSaveHook`1
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using Microsoft.EntityFrameworkCore;

#nullable disable
namespace BuildingBlocks.Persistence.Hooks
{
  public interface IDbSaveHook<TContext> where TContext : DbContext
  {
    void OnBeforeSave(IHookedEntity<TContext> entry);

    void OnAfterSave(IHookedEntity<TContext> entry);

    void OnBeforeSaveCompleted();

    void OnAfterSaveCompleted();
  }
}
