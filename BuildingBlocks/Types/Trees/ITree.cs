// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Types.Trees.ITree`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Types.Trees;

public interface ITree<T>
{
  T Data { get; }

  ITree<T> Parent { get; }

  ICollection<ITree<T>> Children { get; }

  bool IsRoot { get; }

  bool IsLeaf { get; }

  int Level { get; }
}