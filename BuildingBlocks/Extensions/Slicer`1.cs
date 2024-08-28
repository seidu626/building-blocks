// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.Slicer`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
namespace BuildingBlocks.Extensions;

internal sealed class Slicer<T>
{
  private readonly 
#nullable disable
    IEnumerator<T> _iterator;
  private readonly int[] _sizes;
  private volatile bool _hasNext;
  private volatile int _currentSize;
  private volatile int _index;

  public Slicer(IEnumerator<T> iterator, int[] sizes)
  {
    this._iterator = iterator;
    this._sizes = sizes;
    this._index = 0;
    this._currentSize = 0;
    this._hasNext = true;
  }

  public int Index => this._index;

  public IEnumerable<IEnumerable<T>> Slice()
  {
    int length = this._sizes.Length;
    int index = 1;
    int size = 0;
    int i = 0;
    while (this._hasNext)
    {
      if (i < length)
      {
        size = this._sizes[i];
        this._currentSize = size - 1;
      }
      while (this._index < index && this._hasNext)
        this._hasNext = this.MoveNext();
      if (this._hasNext)
      {
        yield return (IEnumerable<T>) new List<T>(this.SliceInternal());
        index += size;
      }
      ++i;
    }
  }

  private IEnumerable<T> SliceInternal()
  {
    if (this._currentSize != -1)
    {
      yield return this._iterator.Current;
      for (int count = 0; count < this._currentSize && this._hasNext; ++count)
      {
        this._hasNext = this.MoveNext();
        if (this._hasNext)
          yield return this._iterator.Current;
      }
    }
  }

  private bool MoveNext()
  {
    ++this._index;
    return this._iterator.MoveNext();
  }
}