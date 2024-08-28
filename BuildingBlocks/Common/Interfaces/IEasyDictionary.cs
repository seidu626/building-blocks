// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Interfaces.IEasyDictionary`2
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections;

namespace BuildingBlocks.Common.Interfaces;

public interface IEasyDictionary<TKey, TValue> : 
  ICollection<TValue>,
  IEnumerable<TValue>,
  IEnumerable
{
  Func<TValue, TKey> KeySelector { get; }

  IEqualityComparer<TKey> Comparer { get; }

  ICollection<TKey> Keys { get; }

  ICollection<TValue> Values { get; }

  TValue this[TKey key] { get; }

  bool ContainsKey(TKey key);

  bool TryGetValue(TKey key, out TValue value);

  bool Remove(TKey key);

  void AddOrReplace(TValue value);
}