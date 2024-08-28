// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.AsyncExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
namespace BuildingBlocks.Extensions;

public static class AsyncExtensions
{
  internal static 
#nullable disable
    Task GetActionTask(Action action, CancellationToken token)
  {
    Task actionTask = new Task(action, token);
    actionTask.Start();
    return actionTask;
  }

  internal static Task<T> GetTask<T>(Func<T> func)
  {
    Task<T> task = new Task<T>(func);
    ((Task) task).Start();
    return task;
  }

  private static Task<T> GetTask<T>(Func<T> func, CancellationToken token)
  {
    Task<T> task = new Task<T>(func, token);
    ((Task) task).Start();
    return task;
  }

  public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
    this IQueryable<TSource> source)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    return source is IAsyncEnumerable<TSource> asyncEnumerable ? asyncEnumerable : (IAsyncEnumerable<TSource>) new AsyncExtensions.AsyncEnumerableAdapter<TSource>(source);
  }

  public static Task ForEachAsync<TSource>(
    this IQueryable<TSource> source,
    Action<TSource> action,
    CancellationToken token = default (CancellationToken))
  {
    return AsyncExtensions.GetActionTask((Action) (() =>
    {
      foreach (TSource source1 in (IEnumerable<TSource>) source)
      {
        if (token.IsCancellationRequested)
          break;
        action(source1);
      }
    }), token);
  }

  public static Task ForEachUntilAsync<TSource>(
    this IQueryable<TSource> source,
    Func<TSource, bool> func,
    CancellationToken token = default (CancellationToken))
  {
    return AsyncExtensions.GetActionTask((Action) (() =>
    {
      foreach (TSource source1 in (IEnumerable<TSource>) source)
      {
        if (token.IsCancellationRequested || !func(source1))
          break;
      }
    }), token);
  }

  public static async Task<List<TSource>> ToListAsync<TSource>(
    this IQueryable<TSource> source,
    CancellationToken token = default (CancellationToken))
  {
    return await AsyncExtensions.GetTask<List<TSource>>((Func<List<TSource>>) (() => ((IEnumerable<TSource>) source).AsEnumerable<TSource>().TakeWhile<TSource>((Func<TSource, bool>) (_ => !token.IsCancellationRequested)).ToList<TSource>()), token);
  }

  public static async Task<TSource[]> ToArrayAsync<TSource>(
    this IQueryable<TSource> source,
    CancellationToken token = default (CancellationToken))
  {
    return await AsyncExtensions.GetTask<TSource[]>((Func<TSource[]>) (() => ((IEnumerable<TSource>) source).AsEnumerable<TSource>().TakeWhile<TSource>((Func<TSource, bool>) (_ => !token.IsCancellationRequested)).ToArray<TSource>()), token);
  }

  public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
    this IQueryable<TSource> source,
    Func<TSource, TKey> keySelector,
    CancellationToken token = default (CancellationToken))
    where TKey : 
#nullable enable
    notnull
  {
    return await AsyncExtensions.GetTask<Dictionary<TKey, TSource>>((Func<Dictionary<TKey, TSource>>) (() => ((IEnumerable<TSource>) source).AsEnumerable<TSource>().TakeWhile<TSource>((Func<TSource, bool>) (_ => !token.IsCancellationRequested)).ToDictionary<TSource, TKey>(keySelector)), token);
  }

  public static async 
#nullable disable
    Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer,
      CancellationToken token = default (CancellationToken))
    where TKey : 
#nullable enable
    notnull
  {
    return await AsyncExtensions.GetTask<Dictionary<TKey, TSource>>((Func<Dictionary<TKey, TSource>>) (() => ((IEnumerable<TSource>) source).AsEnumerable<TSource>().TakeWhile<TSource>((Func<TSource, bool>) (_ => !token.IsCancellationRequested)).ToDictionary<TSource, TKey>(keySelector, comparer)), token);
  }

  public static async 
#nullable disable
    Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      CancellationToken token = default (CancellationToken))
    where TKey : 
#nullable enable
    notnull
  {
    return await AsyncExtensions.GetTask<Dictionary<TKey, TElement>>((Func<Dictionary<TKey, TElement>>) (() => ((IEnumerable<TSource>) source).AsEnumerable<TSource>().TakeWhile<TSource>((Func<TSource, bool>) (_ => !token.IsCancellationRequested)).ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector)), token);
  }

  public static async 
#nullable disable
    Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IEqualityComparer<TKey> comparer,
      CancellationToken token = default (CancellationToken))
    where TKey : 
#nullable enable
    notnull
  {
    return await AsyncExtensions.GetTask<Dictionary<TKey, TElement>>((Func<Dictionary<TKey, TElement>>) (() => ((IEnumerable<TSource>) source).AsEnumerable<TSource>().TakeWhile<TSource>((Func<TSource, bool>) (_ => !token.IsCancellationRequested)).ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, comparer)), token);
  }

  [AttributeUsage(AttributeTargets.Method)]
  internal class ElementAsyncAttribute : Attribute
  {
  }

  private class AsyncEnumerableAdapter<T> : IAsyncEnumerable<
#nullable disable
    T>
  {
    private readonly IQueryable<T> _query;

    public AsyncEnumerableAdapter(IQueryable<T> query)
    {
      this._query = query ?? throw new ArgumentNullException(nameof (query));
    }

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
      return (IAsyncEnumerator<T>) new AsyncEnumeratorImpl<T>(this._query.GetEnumerator(), cancellationToken);
    }
  }
}