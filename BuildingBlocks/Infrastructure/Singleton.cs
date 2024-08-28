// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.Singleton`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Infrastructure;

public class Singleton<T> : BaseSingleton
{
  private static T _instance;

  public static T Instance
  {
    get => Singleton<T>._instance;
    set
    {
      Singleton<T>._instance = value;
      BaseSingleton.AllSingletons[typeof (T)] = (object) value;
    }
  }
}