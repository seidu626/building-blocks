// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.EasyDictionary`2
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections;
using BuildingBlocks.Common.Interfaces;

namespace BuildingBlocks.Common;

public sealed class EasyDictionary<TKey, TValue> : 
  IEasyDictionary<TKey, TValue>,
  ICollection<TValue>,
  IEnumerable<TValue>,
  IEnumerable,
  IReadOnlyDictionary<TKey, TValue>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
  private readonly Dictionary<TKey, TValue> _dictionary;

  public EasyDictionary(
    Func<TValue, TKey> keySelector,
    IEnumerable<TValue> sequence,
    IEqualityComparer<TKey> comparer = null)
    : this(keySelector, comparer: comparer)
  {
    this.PopulateFrom(sequence);
  }

  public EasyDictionary(
    Func<TValue, TKey> keySelector,
    ICollection<TValue> collection,
    IEqualityComparer<TKey> comparer = null)
    : this(keySelector, (uint) collection.Count, comparer)
  {
    this.PopulateFrom((IEnumerable<TValue>) collection);
  }

  public EasyDictionary(
    Func<TValue, TKey> keySelector,
    IDictionary<TKey, TValue> dictionary,
    IEqualityComparer<TKey> comparer = null)
    : this(keySelector, (uint) dictionary.Count, comparer)
  {
    this.PopulateFrom(dictionary);
  }

  public EasyDictionary(
    Func<TValue, TKey> keySelector,
    uint capacity = 0,
    IEqualityComparer<TKey> comparer = null)
  {
    this.KeySelector = Ensure.NotNull<Func<TValue, TKey>>(keySelector, nameof (keySelector));
    this._dictionary = new Dictionary<TKey, TValue>((int) capacity, comparer);
  }

  public Func<TValue, TKey> KeySelector { get; }

  public int Count => this._dictionary.Count;

  public bool IsReadOnly => false;

  public IEqualityComparer<TKey> Comparer => this._dictionary.Comparer;

  public ICollection<TKey> Keys => (ICollection<TKey>) this._dictionary.Keys;

  public ICollection<TValue> Values => (ICollection<TValue>) this._dictionary.Values;

  IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
  {
    get => (IEnumerable<TKey>) this._dictionary.Keys;
  }

  IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
  {
    get => (IEnumerable<TValue>) this._dictionary.Values;
  }

  public TValue this[TKey key] => this._dictionary[key];

  public void Add(TValue value) => this._dictionary.Add(this.KeySelector(value), value);

  public void AddOrReplace(TValue value) => this._dictionary[this.KeySelector(value)] = value;

  public bool Remove(TKey key) => this._dictionary.Remove(key);

  public bool Remove(TValue value) => this._dictionary.Remove(this.KeySelector(value));

  public void Clear() => this._dictionary.Clear();

  public bool Contains(TValue value)
  {
    return this._dictionary.ContainsKey(this.KeySelector(value)) && this._dictionary.ContainsValue(value);
  }

  public bool ContainsKey(TKey key) => this._dictionary.ContainsKey(key);

  public bool TryGetValue(TKey key, out TValue value)
  {
    return this._dictionary.TryGetValue(key, out value);
  }

  public void CopyTo(TValue[] array, int startIndex)
  {
    this._dictionary.Values.CopyTo(array, startIndex);
  }

  IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<TKey, TValue>>) this._dictionary.GetEnumerator();
  }

  public IEnumerator<TValue> GetEnumerator()
  {
    return (IEnumerator<TValue>) this._dictionary.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._dictionary.GetEnumerator();

  private void PopulateFrom(IEnumerable<TValue> sequence)
  {
    switch (sequence)
    {
      case IReadOnlyList<TValue> objList1:
        for (int index = 0; index < objList1.Count; ++index)
          this.Add(objList1[index]);
        break;
      case IList<TValue> objList2:
        for (int index = 0; index < objList2.Count; ++index)
          this.Add(objList2[index]);
        break;
      default:
        using (IEnumerator<TValue> enumerator = sequence.GetEnumerator())
        {
          while (enumerator.MoveNext())
            this.Add(enumerator.Current);
          break;
        }
    }
  }

  private void PopulateFrom(IDictionary<TKey, TValue> dictionary)
  {
    if (dictionary is Dictionary<TKey, TValue> dictionary1)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary1)
        this.Add(keyValuePair.Value);
    }
    else
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) dictionary)
        this.Add(keyValuePair.Value);
    }
  }
}