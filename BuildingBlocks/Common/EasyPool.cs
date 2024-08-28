// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.EasyPool`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.Concurrent;
using BuildingBlocks.Common.Interfaces;

namespace BuildingBlocks.Common;

public sealed class EasyPool<T> : IEasyPool<T>, IDisposable where T : class
{
  private readonly ConcurrentBag<T> _pool;
  private readonly Func<T> _factory;
  private readonly Action<T> _reset;
  private readonly uint _maxCount;

  public EasyPool(Func<T> factory, Action<T> reset, uint maxCount)
  {
    this._factory = Ensure.NotNull<Func<T>>(factory, nameof (factory));
    this._reset = reset;
    this._maxCount = maxCount;
    this._pool = new ConcurrentBag<T>();
  }

  public uint Count => (uint) this._pool.Count;

  public T Rent()
  {
    T result;
    return !this._pool.TryTake(out result) ? this._factory() : result;
  }

  public bool Return(T item, bool reset = true)
  {
    if (reset)
    {
      Action<T> reset1 = this._reset;
      if (reset1 != null)
        reset1(item);
    }
    if ((long) this._pool.Count >= (long) this._maxCount)
      return false;
    this._pool.Add(item);
    return true;
  }

  public void Dispose()
  {
    do
      ;
    while (this._pool.TryTake(out T _));
  }
}