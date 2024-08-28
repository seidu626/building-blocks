// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.AsyncEnumeratorImpl`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks;

internal class AsyncEnumeratorImpl<T> : IAsyncEnumerator<T>, IAsyncDisposable
{
  private readonly IEnumerator<T> _enumerator;
  private readonly CancellationToken _cancellationToken;

  public AsyncEnumeratorImpl(IEnumerator<T> enumerator, CancellationToken cancellationToken)
  {
    this._enumerator = enumerator;
    this._cancellationToken = cancellationToken;
  }

  T IAsyncEnumerator<T>.Current => this._enumerator.Current;

  ValueTask IAsyncDisposable.DisposeAsync()
  {
    this._enumerator.Dispose();
    return ValueTask.CompletedTask;
  }

  ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
  {
    this._cancellationToken.ThrowIfCancellationRequested();
    return new ValueTask<bool>(this._enumerator.MoveNext());
  }
}