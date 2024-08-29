// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.EntityState
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using System;

#nullable disable
namespace BuildingBlocks.Persistence
{
  [Flags]
  public enum EntityState
  {
    Detached = 0,
    Unchanged = 1,
    Added = 4,
    Deleted = 2,
    Modified = Deleted | Unchanged, // 0x00000003
  }
}
