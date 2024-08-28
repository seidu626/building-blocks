// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.EnumerableExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BuildingBlocks.Common;
using BuildingBlocks.Helpers;
using BuildingBlocks.SeedWork;

namespace BuildingBlocks.Extensions;

public static class EnumerableExtensions
{
  [DebuggerStepThrough]
  public static 
#nullable disable
    IEnumerable<T> GetPage<T>(this IEnumerable<T> sequence, uint pageIndex, uint pageSize)
  {
    return sequence.Skip<T>((int) pageIndex * (int) pageSize).Take<T>((int) pageSize);
  }

  [DebuggerStepThrough]
  public static bool NotNullOrEmpty<T>(this IEnumerable<T> sequence)
  {
    return sequence != null && sequence.Any<T>();
  }

  [DebuggerStepThrough]
  public static string ToStringSeparated<T>(this IEnumerable<T> sequence, string separator)
  {
    Ensure.NotNull<IEnumerable<T>>(sequence, nameof (sequence));
    return !sequence.Any<T>() ? string.Empty : string.Join<T>(separator, sequence);
  }

  [DebuggerStepThrough]
  public static string ToCharSeparated<T>(this IEnumerable<T> sequence, char delimiter)
  {
    return sequence.ToStringSeparated<T>(delimiter.ToString());
  }

  [DebuggerStepThrough]
  public static string ToCommaSeparated<T>(this IEnumerable<T> sequence)
  {
    return sequence.ToCharSeparated<T>(',');
  }

  [DebuggerStepThrough]
  public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
  {
    Ensure.NotNull<Action<T>>(action, nameof (action));
    foreach (T obj in sequence)
      action(obj);
  }

  public static T PickRandom<T>(this IEnumerable<T> source)
  {
    try
    {
      if (!(source is T[] source1))
        source1 = source.ToArray<T>();
      return source1[ThreadLocalRandom.Next(((IEnumerable<T>) source1).Count<T>())];
    }
    catch (Exception ex)
    {
      throw ex;
    }
  }

  public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
  {
    return source.Shuffle<T>().Take<T>(count);
  }

  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
  {
    if (!(source is T[] objArray))
      objArray = source.ToArray<T>();
    T[] source1 = objArray;
    int index1 = ((IEnumerable<T>) source1).Count<T>();
    while (index1 > 1)
    {
      --index1;
      int index2 = ThreadLocalRandom.Next(index1 + 1);
      T obj = source1[index2];
      source1[index2] = source1[index1];
      source1[index1] = obj;
    }
    return (IEnumerable<T>) source1;
  }

  [DebuggerStepThrough]
  public static T SelectRandom<T>(this IEnumerable<T> sequence)
  {
    Random random = new Random(Guid.NewGuid().GetHashCode());
    return sequence.SelectRandom<T>(random);
  }

  [DebuggerStepThrough]
  public static T SelectRandom<T>(this IEnumerable<T> sequence, Random random)
  {
    if (sequence is ICollection<T> source)
      return source.ElementAt<T>(random.Next(source.Count));
    int num = 1;
    T obj1 = default (T);
    foreach (T obj2 in sequence)
    {
      if (random.Next(num++) == 0)
        obj1 = obj2;
    }
    return obj1;
  }

  [DebuggerStepThrough]
  public static IEnumerable<T> Randomize<T>(this IEnumerable<T> sequence)
  {
    Ensure.NotNull<IEnumerable<T>>(sequence, nameof (sequence));
    return sequence.Randomize<T>(new Random(Guid.NewGuid().GetHashCode()));
  }

  [DebuggerStepThrough]
  public static IEnumerable<T> Randomize<T>(this IEnumerable<T> sequence, Random random)
  {
    Ensure.NotNull<IEnumerable<T>>(sequence, nameof (sequence));
    Ensure.NotNull<Random>(random, nameof (random));
    T[] buffer = sequence.ToArray<T>();
    for (int i = 0; i < buffer.Length; ++i)
    {
      int j = random.Next(i, buffer.Length);
      yield return buffer[j];
      buffer[j] = buffer[i];
    }
  }

  [DebuggerStepThrough]
  public static IEnumerable<T> DistinctBy<T, TKey>(
    this IEnumerable<T> sequence,
    Func<T, TKey> selector)
  {
    return sequence.DistinctBy<T, TKey>(selector, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default);
  }

  [DebuggerStepThrough]
  public static IEnumerable<T> DistinctBy<T, TKey>(
    this IEnumerable<T> sequence,
    Func<T, TKey> selector,
    IEqualityComparer<TKey> comparer)
  {
    HashSet<TKey> keys = new HashSet<TKey>(comparer);
    foreach (T obj in sequence)
    {
      if (keys.Add(selector(obj)))
        yield return obj;
    }
  }

  [DebuggerStepThrough]
  public static EasyDictionary<TKey, TValue> ToEasyDictionary<TKey, TValue>(
    this IEnumerable<TValue> sequence,
    Func<TValue, TKey> keySelector)
  {
    return sequence.ToEasyDictionary<TKey, TValue>(keySelector, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default);
  }

  [DebuggerStepThrough]
  public static EasyDictionary<TKey, TValue> ToEasyDictionary<TKey, TValue>(
    this IEnumerable<TValue> sequence,
    Func<TValue, TKey> keySelector,
    IEqualityComparer<TKey> comparer)
  {
    return new EasyDictionary<TKey, TValue>(keySelector, sequence, comparer);
  }

  [DebuggerStepThrough]
  public static IList<T> SpeculativeToList<T>(this IEnumerable<T> sequence)
  {
    return sequence is IList<T> objList ? objList : (IList<T>) sequence.ToList<T>();
  }

  [DebuggerStepThrough]
  public static IReadOnlyList<T> SpeculativeToReadOnlyList<T>(this IEnumerable<T> sequence)
  {
    return sequence is IReadOnlyList<T> objList ? objList : (IReadOnlyList<T>) sequence.ToList<T>();
  }

  [DebuggerStepThrough]
  public static T[] SpeculativeToArray<T>(this IEnumerable<T> sequence)
  {
    return sequence is T[] objArray ? objArray : sequence.ToArray<T>();
  }

  [DebuggerStepThrough]
  public static IEnumerable<T> ToReadOnlySequence<T>(this IEnumerable<T> sequence)
  {
    Ensure.NotNull<IEnumerable<T>>(sequence, nameof (sequence));
    return !(sequence is IReadOnlyList<T>) ? sequence.Skip<T>(0) : sequence;
  }

  [DebuggerStepThrough]
  public static IEnumerable<T> HandleExceptionWhenYieldReturning<T>(
    this IEnumerable<T> sequence,
    Func<Exception, bool> exceptionPredicate,
    Action<Exception> actionToExecuteOnException)
  {
    Ensure.NotNull<Func<Exception, bool>>(exceptionPredicate, nameof (exceptionPredicate));
    Ensure.NotNull<Action<Exception>>(actionToExecuteOnException, nameof (actionToExecuteOnException));
    IEnumerator<T> enumerator = sequence.GetEnumerator();
    while (true)
    {
      T current;
      try
      {
        if (enumerator.MoveNext())
          current = enumerator.Current;
        else
          break;
      }
      catch (Exception ex)
      {
        if (exceptionPredicate(ex))
        {
          actionToExecuteOnException(ex);
          yield break;
        }
        else
          throw;
      }
      yield return current;
    }
    enumerator.Dispose();
  }

  [DebuggerStepThrough]
  public static IEnumerable<TSource[]> Batch<TSource>(this IEnumerable<TSource> source, uint size)
  {
    Ensure.NotNull<IEnumerable<TSource>>(source, nameof (source));
    Ensure.That<ArgumentOutOfRangeException>(size > 0U, nameof (size));
    TSource[] array = (TSource[]) null;
    int newSize = 0;
    if (source is IReadOnlyList<TSource> indexibale)
    {
      for (int i = 0; i < indexibale.Count; ++i)
      {
        if (array == null)
          array = new TSource[(int) size];
        TSource source1 = indexibale[i];
        array[newSize++] = source1;
        if ((long) newSize == (long) size)
        {
          yield return array;
          array = (TSource[]) null;
          newSize = 0;
        }
      }
    }
    else
    {
      foreach (TSource source2 in source)
      {
        if (array == null)
          array = new TSource[(int) size];
        array[newSize++] = source2;
        if ((long) newSize == (long) size)
        {
          yield return array;
          array = (TSource[]) null;
          newSize = 0;
        }
      }
    }
    if (array != null && newSize > 0)
    {
      Array.Resize<TSource>(ref array, newSize);
      yield return array;
    }
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Each<T>(this IEnumerable<T> source, Action<T> action)
  {
    foreach (T obj in source)
      action(obj);
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Each<T>(this IEnumerable<T> source, Action<T, int> action)
  {
    int num = 0;
    foreach (T obj in source)
      action(obj, num++);
  }

  public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> source)
  {
    if (source == null || !source.Any<T>())
      return EnumerableExtensions.DefaultReadOnlyCollection<T>.Empty;
    switch (source)
    {
      case ReadOnlyCollection<T> readOnlyCollection:
        return readOnlyCollection;
      case List<T> objList:
        return objList.AsReadOnly();
      default:
        return new ReadOnlyCollection<T>((IList<T>) source.ToList<T>());
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Dictionary<TKey, TSource> ToDictionarySafe<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector)
  {
    return source.ToDictionarySafe<TSource, TKey, TSource>(keySelector, (Func<TSource, TSource>) (src => src), (IEqualityComparer<TKey>) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Dictionary<TKey, TSource> ToDictionarySafe<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    IEqualityComparer<TKey> comparer)
  {
    return source.ToDictionarySafe<TSource, TKey, TSource>(keySelector, (Func<TSource, TSource>) (src => src), comparer);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Dictionary<TKey, TElement> ToDictionarySafe<TSource, TKey, TElement>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TElement> elementSelector)
  {
    return source.ToDictionarySafe<TSource, TKey, TElement>(keySelector, elementSelector, (IEqualityComparer<TKey>) null);
  }

  public static Dictionary<TKey, TElement> ToDictionarySafe<TSource, TKey, TElement>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TElement> elementSelector,
    IEqualityComparer<TKey> comparer)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    if (keySelector == null)
      throw new ArgumentNullException(nameof (keySelector));
    if (elementSelector == null)
      throw new ArgumentNullException(nameof (elementSelector));
    Dictionary<TKey, TElement> dictionarySafe = new Dictionary<TKey, TElement>(comparer);
    foreach (TSource source1 in source)
      dictionarySafe[keySelector(source1)] = elementSelector(source1);
    return dictionarySafe;
  }

  public static IEnumerable<TEntity> OrderBySequence<TEntity>(
    this IEnumerable<TEntity> source,
    IEnumerable<int> ids)
    where TEntity : Entity
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    if (ids == null)
      throw new ArgumentNullException(nameof (ids));
    return ids.Join<int, TEntity, int, TEntity>(source, (Func<int, int>) (id => id), (Func<TEntity, int>) (entity => entity.Id), (Func<int, TEntity, TEntity>) ((id, entity) => entity));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string StrJoin(this IEnumerable<string> source, string separator)
  {
    return string.Join(separator, source);
  }

  private static class DefaultReadOnlyCollection<T>
  {
    private static ReadOnlyCollection<T> defaultCollection;

    internal static ReadOnlyCollection<T> Empty
    {
      get
      {
        if (EnumerableExtensions.DefaultReadOnlyCollection<T>.defaultCollection == null)
          EnumerableExtensions.DefaultReadOnlyCollection<T>.defaultCollection = new ReadOnlyCollection<T>((IList<T>) new T[0]);
        return EnumerableExtensions.DefaultReadOnlyCollection<T>.defaultCollection;
      }
    }
  }
}