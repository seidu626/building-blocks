﻿// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.SingletonList`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Infrastructure;

public class SingletonList<T> : Singleton<IList<T>>
{
  static SingletonList() => Singleton<IList<T>>.Instance = (IList<T>) new List<T>();

  public new static IList<T> Instance => Singleton<IList<T>>.Instance;
}