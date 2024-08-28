using BuildingBlocks.Types;

namespace BuildingBlocks.Extensions;

/// <summary>
/// Represents model extensions
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Convert list to paged list according to paging request
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="list">List of objects</param>
    /// <param name="pagingRequestModel">Paging request model</param>
    /// <returns>Paged list</returns>
    public static IPagedList<T> ToPagedList<T>(this IList<T> list, IPagingRequestModel pagingRequestModel)
    {
        return new PagedList<T>(list, pagingRequestModel.Page - 1, pagingRequestModel.PageSize);
    }

    /// <summary>
    /// Convert async-enumerable sequence to paged list according to paging request
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="collection">Async-enumerable sequence of objects</param>
    /// <param name="pagingRequestModel">Paging request model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list
    /// </returns>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(
        this IAsyncEnumerable<T> collection,
        IPagingRequestModel pagingRequestModel)
    {
        // return (await collection.ToListAsync<T>()).ToPagedList<T>(pagingRequestModel);
        return new PagedList<T>();
    }
}