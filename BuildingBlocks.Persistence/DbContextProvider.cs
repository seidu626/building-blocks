// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.DbContextProvider
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using Microsoft.EntityFrameworkCore;
using System;

#nullable disable
namespace BuildingBlocks.Persistence
{
  public class DbContextProvider : IDisposable
  {
    private bool _disposed;
    private readonly Func<DbContext> _instanceFunc;
    private DbContext _dbContext;

    public DbContext DbContext => this._dbContext ?? (this._dbContext = this._instanceFunc());

    public DbContextProvider(Func<DbContext> dbContextFactory)
    {
      this._instanceFunc = dbContextFactory;
    }

    public void Dispose()
    {
      if (this._disposed || this._dbContext == null)
        return;
      this._disposed = true;
      this._dbContext.Dispose();
    }
  }
}
