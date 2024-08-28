// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Types.Trees.TreeExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
namespace BuildingBlocks.Types.Trees;

public static class TreeExtensions
{
  public static 
#nullable disable
    IEnumerable<TNode> Flatten<TNode>(
      this IEnumerable<TNode> nodes,
      Func<TNode, IEnumerable<TNode>> childrenSelector)
  {
    if (nodes == null)
      throw new ArgumentNullException(nameof (nodes));
    if (!(nodes is TNode[] nodeArray1))
      nodeArray1 = nodes.ToArray<TNode>();
    TNode[] nodeArray2 = nodeArray1;
    return ((IEnumerable<TNode>) nodeArray2).SelectMany<TNode, TNode>((Func<TNode, IEnumerable<TNode>>) (c => childrenSelector(c).Flatten<TNode>(childrenSelector))).Concat<TNode>((IEnumerable<TNode>) nodeArray2);
  }

  public static ITree<T> ToTree<T>(this IList<T> items, Func<T, T, bool> parentSelector)
  {
    if (items == null)
      throw new ArgumentNullException(nameof (items));
    return (ITree<T>) Tree<T>.FromLookup(items.ToLookup<T, T, T>((Func<T, T>) (item => ((IEnumerable<T>) items).FirstOrDefault<T>((Func<T, bool>) (parent => parentSelector(parent, item)))), (Func<T, T>) (child => child)));
  }
}