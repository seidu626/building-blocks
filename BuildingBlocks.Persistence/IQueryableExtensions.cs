// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.IQueryableExtensions
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using BuildingBlocks.Exceptions;
using BuildingBlocks.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace BuildingBlocks.Persistence
{
  public static class IQueryableExtensions
  {
    public static IQueryable<T> Expand<T, TId>(this IQueryable<T> query, string path) where T : Entity<TId>
    {
      Guard.AgainstNull(query, nameof(query));
      Guard.AgainstNullOrEmpty(path, nameof(path));
      return EntityFrameworkQueryableExtensions.Include<T>(query, path);
    }

    public static IQueryable<T> Expand<T, TId, TProperty>(
      this IQueryable<T> query,
      Expression<Func<T, TProperty>> path)
      where T : Entity<TId>
    {
      Guard.AgainstNull(query, nameof(query));
      Guard.AgainstNull(path, nameof(path));
      return EntityFrameworkQueryableExtensions.Include<T, TProperty>(query, path);
    }
  }
}
