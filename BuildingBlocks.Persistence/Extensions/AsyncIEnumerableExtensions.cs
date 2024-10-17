using BuildingBlocks.Types;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.Extensions;

public static class AsyncIEnumerableExtensions
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="source">source</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize,
        bool getOnlyTotalCount = false)
    {
        if (source == null)
            return new PagedList<T>(new List<T>(), pageIndex, pageSize);

        //min allowed page size is 1
        pageSize = Math.Max(pageSize, 1);

        var count = await source.CountAsync();

        var data = new List<T>();

        if (!getOnlyTotalCount)
            data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync());

        return new PagedList<T>(data, pageIndex, pageSize, count);
    }

    public static IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TResult>> predicate)
    {
        return source.ToAsyncEnumerable().SelectAwait(predicate);
    }

    public static Task<TSource> FirstOrDefaultAwaitAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().FirstOrDefaultAwaitAsync(predicate).AsTask();
    }

    public static Task<bool> AllAwaitAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().AllAwaitAsync(predicate).AsTask();
    }

    public static IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Task<IList<TResult>>> predicate)
    {
        return source.ToAsyncEnumerable().SelectManyAwait(async item =>
            (await predicate(item)).ToAsyncEnumerable());
    }

    public static IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Task<IEnumerable<TResult>>> predicate)
    {
        return source.ToAsyncEnumerable().SelectManyAwait(async item =>
            (await predicate(item)).ToAsyncEnumerable());
    }

    public static IAsyncEnumerable<TSource> WhereAwait<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().WhereAwait(predicate);
    }

    public static Task<bool> AnyAwaitAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().AnyAwaitAsync(predicate).AsTask();
    }

    public static Task<TSource> SingleOrDefaultAwaitAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().SingleOrDefaultAwaitAsync(predicate).AsTask();
    }

    public static Task<List<TSource>> ToListAsync<TSource>(this IEnumerable<TSource> source)
    {
        return source.ToAsyncEnumerable().ToListAsync().AsTask();
    }

    public static IOrderedAsyncEnumerable<TSource> OrderByDescendingAwait<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TKey>> keySelector)
    {
        return source.ToAsyncEnumerable().OrderByDescendingAwait(keySelector);
    }

    public static IAsyncEnumerable<IAsyncGrouping<TKey, TElement>> GroupByAwait<TSource, TKey, TElement>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TKey>> keySelector,
        Func<TSource, ValueTask<TElement>> elementSelector)
    {
        return source.ToAsyncEnumerable().GroupByAwait(keySelector, elementSelector);
    }

    public static ValueTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator)
    {
        return source.ToAsyncEnumerable().AggregateAwaitAsync(seed, accumulator);
    }

    public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TKey>> keySelector,
        Func<TSource, ValueTask<TElement>> elementSelector)
        where TKey : notnull
    {
        return source.ToAsyncEnumerable().ToDictionaryAwaitAsync(keySelector, elementSelector);
    }

    public static IAsyncEnumerable<IAsyncGrouping<TKey, TSource>> GroupByAwait<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TKey>> keySelector)
    {
        return source.ToAsyncEnumerable().GroupByAwait(keySelector);
    }

    public static ValueTask<decimal> SumAwaitAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<decimal>> selector)
    {
        return source.ToAsyncEnumerable().SumAwaitAsync(selector);
    }
}