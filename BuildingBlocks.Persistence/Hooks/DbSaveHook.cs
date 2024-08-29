// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Hooks.DbSaveHook`2
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using Microsoft.EntityFrameworkCore;
using System;

#nullable disable
namespace BuildingBlocks.Persistence.Hooks
{
  public abstract class DbSaveHook<TContext, TEntity> : IDbSaveHook<TContext>
    where TContext : DbContext
    where TEntity : class
  {
    public virtual void OnBeforeSave(IHookedEntity<TContext> entry)
    {
      TEntity entity = entry.Entity as TEntity;
      switch (entry.InitialState)
      {
        case EntityState.Deleted:
          this.OnDeleting(entity, entry);
          break;
        case EntityState.Modified:
          this.OnUpdating(entity, entry);
          break;
        case EntityState.Added:
          this.OnInserting(entity, entry);
          break;
      }
    }

    protected virtual void OnInserting(TEntity entity, IHookedEntity<TContext> entry)
    {
      throw new NotImplementedException();
    }

    protected virtual void OnUpdating(TEntity entity, IHookedEntity<TContext> entry)
    {
      throw new NotImplementedException();
    }

    protected virtual void OnDeleting(TEntity entity, IHookedEntity<TContext> entry)
    {
      throw new NotImplementedException();
    }

    public virtual void OnBeforeSaveCompleted()
    {
    }

    public virtual void OnAfterSave(IHookedEntity<TContext> entry)
    {
      TEntity entity = entry.Entity as TEntity;
      switch (entry.InitialState)
      {
        case EntityState.Deleted:
          this.OnDeleted(entity, entry);
          break;
        case EntityState.Modified:
          this.OnUpdated(entity, entry);
          break;
        case EntityState.Added:
          this.OnInserted(entity, entry);
          break;
      }
    }

    protected virtual void OnInserted(TEntity entity, IHookedEntity<TContext> entry)
    {
      throw new NotImplementedException();
    }

    protected virtual void OnUpdated(TEntity entity, IHookedEntity<TContext> entry)
    {
      throw new NotImplementedException();
    }

    protected virtual void OnDeleted(TEntity entity, IHookedEntity<TContext> entry)
    {
      throw new NotImplementedException();
    }

    public virtual void OnAfterSaveCompleted()
    {
    }
  }
}
