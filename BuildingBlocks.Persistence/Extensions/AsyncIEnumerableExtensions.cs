// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Extensions.AsyncIEnumerableExtensions
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace BuildingBlocks.Persistence.Extensions
{
  public static class AsyncIEnumerableExtensions
  {
    public static 
    #nullable disable
    IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<TResult>> predicate)
    {
      return AsyncEnumerable.SelectAwait<TSource, TResult>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), predicate);
    }

    public static Task<TSource> FirstOrDefaultAwaitAsync<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<bool>> predicate)
    {
      return AsyncEnumerable.FirstOrDefaultAwaitAsync<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), predicate, new CancellationToken()).AsTask();
    }

    public static Task<bool> AllAwaitAsync<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<bool>> predicate)
    {
      return AsyncEnumerable.AllAwaitAsync<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), predicate, new CancellationToken()).AsTask();
    }

    public static IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(
      this IEnumerable<TSource> source,
      Func<TSource, Task<IList<TResult>>> predicate)
    {
      return AsyncEnumerable.SelectManyAwait<TSource, TResult>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), new Func<TSource, ValueTask<IAsyncEnumerable<TResult>>>(getAsyncEnumerable));

      async ValueTask<IAsyncEnumerable<TResult>> getAsyncEnumerable(TSource items)
      {
        return AsyncEnumerable.ToAsyncEnumerable<TResult>((IEnumerable<TResult>) await predicate(items));
      }
    }

    public static IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(
      this IEnumerable<TSource> source,
      Func<TSource, Task<IEnumerable<TResult>>> predicate)
    {
      return AsyncEnumerable.SelectManyAwait<TSource, TResult>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), new Func<TSource, ValueTask<IAsyncEnumerable<TResult>>>(getAsyncEnumerable));

      async ValueTask<IAsyncEnumerable<TResult>> getAsyncEnumerable(TSource items)
      {
        return AsyncEnumerable.ToAsyncEnumerable<TResult>(await predicate(items));
      }
    }

    public static IAsyncEnumerable<TSource> WhereAwait<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<bool>> predicate)
    {
      return AsyncEnumerable.WhereAwait<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), predicate);
    }

    public static Task<bool> AnyAwaitAsync<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<bool>> predicate)
    {
      return AsyncEnumerable.AnyAwaitAsync<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), predicate, new CancellationToken()).AsTask();
    }

    public static Task<TSource> SingleOrDefaultAwaitAsync<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<bool>> predicate)
    {
      return AsyncEnumerable.SingleOrDefaultAwaitAsync<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), predicate, new CancellationToken()).AsTask();
    }

    public static Task<List<TSource>> ToListAsync<TSource>(this IEnumerable<TSource> source)
    {
      return AsyncEnumerable.ToListAsync<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), new CancellationToken()).AsTask();
    }

    public static IOrderedAsyncEnumerable<TSource> OrderByDescendingAwait<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<TKey>> keySelector)
    {
      return AsyncEnumerable.OrderByDescendingAwait<TSource, TKey>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), keySelector);
    }

    public static IAsyncEnumerable<IAsyncGrouping<TKey, TElement>> GroupByAwait<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<TKey>> keySelector,
      Func<TSource, ValueTask<TElement>> elementSelector)
    {
      return AsyncEnumerable.GroupByAwait<TSource, TKey, TElement>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), keySelector, elementSelector);
    }

    public static ValueTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(
      this IEnumerable<TSource> source,
      TAccumulate seed,
      Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator)
    {
      return AsyncEnumerable.AggregateAwaitAsync<TSource, TAccumulate>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), seed, accumulator, new CancellationToken());
    }

    public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<TKey>> keySelector,
      Func<TSource, ValueTask<TElement>> elementSelector)
      where TKey : 
      #nullable enable
      notnull
    {
      return AsyncEnumerable.ToDictionaryAwaitAsync<TSource, TKey, TElement>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), keySelector, elementSelector, new CancellationToken());
    }

    public static 
    #nullable disable
    IAsyncEnumerable<IAsyncGrouping<TKey, TSource>> GroupByAwait<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<TKey>> keySelector)
    {
      return AsyncEnumerable.GroupByAwait<TSource, TKey>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), keySelector);
    }

    public static ValueTask<Decimal> SumAwaitAsync<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ValueTask<Decimal>> selector)
    {
      return AsyncEnumerable.SumAwaitAsync<TSource>(AsyncEnumerable.ToAsyncEnumerable<TSource>(source), selector, new CancellationToken());
    }
  }
}
