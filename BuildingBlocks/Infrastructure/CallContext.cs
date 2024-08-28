// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.CallContext`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.Concurrent;

namespace BuildingBlocks.Infrastructure;

public static class CallContext<T>
{
  private static readonly ConcurrentDictionary<string, AsyncLocal<T>> State = new ConcurrentDictionary<string, AsyncLocal<T>>();

  public static void SetData(string name, T data)
  {
    CallContext<T>.State.GetOrAdd(name, (Func<string, AsyncLocal<T>>) (_ => new AsyncLocal<T>())).Value = data;
  }

  public static T GetData(string name)
  {
    AsyncLocal<T> asyncLocal;
    return !CallContext<T>.State.TryGetValue(name, out asyncLocal) ? default (T) : asyncLocal.Value;
  }

  public static void FreeNamedDataSlot(string key)
  {
    CallContext<T>.State.TryRemove(key, out AsyncLocal<T> _);
  }
}