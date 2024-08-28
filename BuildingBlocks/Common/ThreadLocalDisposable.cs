// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ThreadLocalDisposable`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.Concurrent;

namespace BuildingBlocks.Common;

public sealed class ThreadLocalDisposable<T> : IDisposable where T : IDisposable
{
  private readonly ConcurrentBag<T> _values;
  private readonly ThreadLocal<T> _threadLocal;

  public ThreadLocalDisposable(Func<T> valueFactory)
  {
    ThreadLocalDisposable<T> threadLocalDisposable = this;
    this._values = new ConcurrentBag<T>();
    this._threadLocal = new ThreadLocal<T>((Func<T>) (() =>
    {
      T obj = valueFactory();
      threadLocalDisposable._values.Add(obj);
      return obj;
    }));
  }

  public bool IsValueCreated => this._threadLocal.IsValueCreated;

  public T Value => this._threadLocal.Value;

  public override string ToString() => this._threadLocal.ToString();

  public void Dispose()
  {
    this._threadLocal.Dispose();
    Array.ForEach<T>(this._values.ToArray(), (Action<T>) (v => v.Dispose()));
  }
}