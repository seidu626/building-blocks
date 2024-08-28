// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Types.Trees.Tree`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
namespace BuildingBlocks.Types.Trees;

internal record Tree<T> : ITree<
#nullable disable
  T>
{
  private Tree(T data)
  {
    this.Children = (ICollection<ITree<T>>) new LinkedList<ITree<T>>();
    this.Data = data;
  }

  public T Data { get; }

  public ITree<T> Parent { get; private set; }

  public ICollection<ITree<T>> Children { get; }

  public bool IsRoot => this.Parent == null;

  public bool IsLeaf => this.Children.Count == 0;

  public int Level => !this.IsRoot ? this.Parent.Level + 1 : 0;

  public static Tree<T> FromLookup(ILookup<T, T> lookup)
  {
    Tree<T> tree = new Tree<T>(lookup.Count == 1 ? lookup.First<IGrouping<T, T>>().Key : default (T));
    tree.LoadChildren(lookup);
    return tree;
  }

  private void LoadChildren(ILookup<T, T> lookup)
  {
    foreach (T data in lookup[this.Data])
    {
      Tree<T> tree = new Tree<T>(data)
      {
        Parent = (ITree<T>) this
      };
      this.Children.Add((ITree<T>) tree);
      tree.LoadChildren(lookup);
    }
  }

}