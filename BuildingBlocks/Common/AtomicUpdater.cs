// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.AtomicUpdater`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;

namespace BuildingBlocks.Common;

public sealed class AtomicUpdater<T> where T : class
{
  private T _primary;
  private T _secondary;

  [DebuggerStepThrough]
  public AtomicUpdater(T initialPrimary, T initialSecondary)
  {
    this._primary = (object) initialPrimary != (object) initialSecondary ? initialPrimary : throw new InvalidOperationException("initialPrimary and initialSecondary should not be the same object.");
    this._secondary = initialSecondary;
  }

  public T Value => AtomicUpdater<T>.ReadFresh(ref this._primary);

  [DebuggerStepThrough]
  public void Update(Func<T, T> updater)
  {
    T obj1 = AtomicUpdater<T>.ReadFresh(ref this._secondary);
    T obj2 = Interlocked.Exchange<T>(ref this._primary, updater(obj1));
    this._secondary = Interlocked.Exchange<T>(ref this._primary, updater(obj2));
  }

  private static T ReadFresh(ref T location)
  {
    return Interlocked.CompareExchange<T>(ref location, default (T), default (T));
  }
}