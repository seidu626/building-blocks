// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.TreeNode`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Extensions;

public sealed class TreeNode<T>
{
  public T Value { get; }

  public TreeNode<T>[] Children { get; }

  public TreeNode(T value, TreeNode<T>[] children)
  {
    this.Value = value;
    this.Children = children;
  }
}