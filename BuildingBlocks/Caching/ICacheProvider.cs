using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Caching
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Sets the default cached key.
        /// </summary>
        /// <param name="key">The key to be set as default.</param>
        /// <returns>The default cached key.</returns>
        string? SetDefaultCachedKey(string? key);

        /// <summary>
        /// Sets cached keys asynchronously.
        /// </summary>
        /// <param name="record">The data to be cached.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="options"></param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetCachedKeysAsync(string record,
            string? key,
            DistributedCacheEntryOptions options,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets a cached item asynchronously, using a semaphore to control access.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="options"></param>
        /// <param name="semaphore">The semaphore to control access.</param>
        /// <param name="func">The function to retrieve the item if it is not in the cache.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The cached item or the result of the function if not cached.</returns>
        Task<T> GetAsync<T>(string? key,
            DistributedCacheEntryOptions options,
            SemaphoreSlim semaphore,
            Func<Task<T>> func,
            CancellationToken cancellationToken)
            where T : class;

        /// <summary>
        /// Gets a cached item asynchronously, using a semaphore to control access.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="options"></param>
        /// <param name="semaphore">The semaphore to control access.</param>
        /// <param name="func">The function to retrieve the item if it is not in the cache.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The cached item or the result of the function if not cached.</returns>
        Task<T> GetAsync<T>(string? key,
            DistributedCacheEntryOptions options,
            SemaphoreSlim semaphore,
            Func<T> func,
            CancellationToken cancellationToken)
            where T : class;

        /// <summary>
        /// Gets a cached item asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The cached item.</returns>
        Task<T> GetAsync<T>(string? key, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Sets a cached item asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the item to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="options">The cache entry options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(
            string? key,
            T value,
            DistributedCacheEntryOptions options,
            CancellationToken cancellationToken)
            where T : class;

        /// <summary>
        /// Clears a cached item asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearAsync(string? key);

        /// <summary>
        /// Flushes all cached items asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, returning a list of flushed keys.</returns>
        Task<List<string?>> FlushAsync(CancellationToken cancellationToken);
    }
}
