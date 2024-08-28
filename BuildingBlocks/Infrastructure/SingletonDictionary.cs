// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.SingletonDictionary`2
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Infrastructure;

public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
{
  static SingletonDictionary()
  {
    Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
  }

  public new static IDictionary<TKey, TValue> Instance
  {
    get => (IDictionary<TKey, TValue>) Singleton<Dictionary<TKey, TValue>>.Instance;
  }
}