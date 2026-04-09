// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Hooks.NullDbHookHandler`1
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BuildingBlocks.Persistence.Hooks
{
  public sealed class NullDbHookHandler<TContext, TId> : IDbHookHandler<TContext, TId> where TContext : DbContext
  {
    private static readonly IDbHookHandler<TContext, TId> s_instance = new NullDbHookHandler<TContext, TId>();

    public static IDbHookHandler<TContext, TId> Instance => s_instance;

    public bool HasImportantSaveHooks() => false;

    public IEnumerable<IDbSaveHook<TContext, TId>> TriggerPreSaveHooks(
      IEnumerable<IHookedEntity<TContext, TId>> entries,
      bool importantHooksOnly,
      out bool anyStateChanged)
    {
      anyStateChanged = false;
      return Enumerable.Empty<IDbSaveHook<TContext, TId>>();
    }

    public IEnumerable<IDbSaveHook<TContext, TId>> TriggerPostSaveHooks(
      IEnumerable<IHookedEntity<TContext, TId>> entries,
      bool importantHooksOnly)
    {
      return Enumerable.Empty<IDbSaveHook<TContext, TId>>();
    }
  }
}
