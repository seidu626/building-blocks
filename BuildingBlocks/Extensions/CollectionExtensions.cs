// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.CollectionExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.CompilerServices;
using BuildingBlocks.Types.Collections;

namespace BuildingBlocks.Extensions;

public static class CollectionExtensions
{
  public static void AddRange<T>(this ICollection<T> initial, IEnumerable<T> other)
  {
    if (other == null)
      return;
    if (initial is List<T> objList)
    {
      objList.AddRange(other);
    }
    else
    {
      foreach (T obj in other)
        initial.Add(obj);
    }
  }

  public static SyncedCollection<T> AsSynchronized<T>(this ICollection<T> source)
  {
    return source.AsSynchronized<T>(new object());
  }

  public static SyncedCollection<T> AsSynchronized<T>(this ICollection<T> source, object syncRoot)
  {
    return source is SyncedCollection<T> syncedCollection ? syncedCollection : new SyncedCollection<T>(source, syncRoot);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsNullOrEmpty<T>(this ICollection<T> source)
  {
    return source == null || source.Count == 0;
  }

  public static TreeNode<T>[] ToTree<T, TId>(
    this ICollection<T> collection,
    Func<T, TId> idSelector,
    Func<T, TId> parentIdSelector,
    TId rootId = default)
  {
    return collection.Where<T>((Func<T, bool>) (x => EqualityComparer<TId>.Default.Equals(parentIdSelector(x), rootId))).Select<T, TreeNode<T>>((Func<T, TreeNode<T>>) (x => new TreeNode<T>(x, collection.ToTree<T, TId>(idSelector, parentIdSelector, idSelector(x))))).ToArray<TreeNode<T>>();
  }
}