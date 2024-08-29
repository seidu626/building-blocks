// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Hooks.HookMetadata
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;

#nullable disable
namespace BuildingBlocks.Persistence.Hooks
{
  public class HookMetadata
  {
    public Type HookedType { get; set; }

    public Type ImplType { get; set; }

    public Type DbContextType { get; set; }

    public bool Important { get; set; }

    public static HookMetadata Create<THook, TContext>(Type hookedType, bool important = false)
      where THook : IDbSaveHook<TContext>
      where TContext : DbContext
    {
      Guard.AgainstNull((object) hookedType, nameof (hookedType));
      return new HookMetadata()
      {
        ImplType = typeof (THook),
        DbContextType = typeof (TContext),
        HookedType = hookedType,
        Important = important
      };
    }
  }
}
