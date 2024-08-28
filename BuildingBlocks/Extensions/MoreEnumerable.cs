// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.MoreEnumerable
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Extensions;

public static class MoreEnumerable
{
  public static TSource MinBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> selector)
  {
    return source.MinBy<TSource, TKey>(selector, (IComparer<TKey>) Comparer<TKey>.Default);
  }

  public static TSource PopMinBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> selector)
  {
    if (!(source is IList<TSource> source1))
      source1 = (IList<TSource>) source.ToList<TSource>();
    TSource source2 = source1.MinBy<TSource, TKey>(selector, (IComparer<TKey>) Comparer<TKey>.Default);
    source1.ToList<TSource>().Remove(source2);
    return source2;
  }

  public static TSource MinBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> selector,
    IComparer<TKey> comparer)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    if (selector == null)
      throw new ArgumentNullException(nameof (selector));
    if (comparer == null)
      throw new ArgumentNullException(nameof (comparer));
    using (IEnumerator<TSource> enumerator = source.GetEnumerator())
    {
      TSource source1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no elements");
      TKey y = selector(source1);
      while (enumerator.MoveNext())
      {
        TSource current = enumerator.Current;
        TKey x = selector(current);
        if (comparer.Compare(x, y) < 0)
        {
          source1 = current;
          y = x;
        }
      }
      return source1;
    }
  }

  public static T PopValue<T>(this IEnumerable<T> items, T item)
  {
    if (!(items is IList<T> objList))
      objList = (IList<T>) items.ToList<T>();
    T obj = item;
    objList.Remove(obj);
    return obj;
  }

  public static T PopValue<T>(this IEnumerable<T> items, Predicate<T> predicate)
  {
    if (!(items is IList<T> objList))
      objList = (IList<T>) items.ToList<T>();
    IList<T> items1 = objList;
    for (int index = items1.Count - 1; index >= 0; --index)
    {
      if (predicate(items1[index]))
        return items1.PopValue<T>(items1[index]);
    }
    return default (T);
  }

  public static List<T> PopValues<T>(this IEnumerable<T> items, IEnumerable<T> itemList)
  {
    List<T> objList1 = new List<T>();
    if (!(items is IList<T> objList2))
      objList2 = (IList<T>) items.ToList<T>();
    IList<T> objList3 = objList2;
    if (!(itemList is IList<T> source))
      source = (IList<T>) itemList.ToList<T>();
    foreach (T obj in source.Reverse<T>())
    {
      objList1.Add(obj);
      objList3.Remove(obj);
    }
    return objList1;
  }

  public static List<T> PopValues<T>(this IEnumerable<T> items, Predicate<T> predicate)
  {
    List<T> objList1 = new List<T>();
    if (!(items is IList<T> objList2))
      objList2 = (IList<T>) items.ToList<T>();
    IList<T> objList3 = objList2;
    for (int index = objList3.Count - 1; index >= 0; --index)
    {
      if (predicate(objList3[index]))
      {
        objList1.Add(objList3[index]);
        objList3.Remove(objList3[index]);
      }
    }
    return objList1;
  }
}