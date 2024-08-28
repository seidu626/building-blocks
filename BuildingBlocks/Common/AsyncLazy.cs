// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.AsyncLazy`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Common;

public sealed class AsyncLazy<T> : Lazy<Task<T>>
{
  public AsyncLazy(Func<T> valueFactory)
    : base((Func<Task<T>>) (() => Task.Factory.StartNew<T>(valueFactory)))
  {
  }

  public AsyncLazy(Func<Task<T>> taskFactory)
    : base((Func<Task<T>>) (() => TaskExtensions.Unwrap<T>(Task.Factory.StartNew<Task<T>>(taskFactory))))
  {
  }

  public TaskAwaiter<T> GetAwaiter() => this.Value.GetAwaiter();
}