namespace BuildingBlocks.Caching
{
    public interface IStaticCacheManager : ICacheKeyService, IDisposable
    {
        /// <summary>
        /// Gets a cached item asynchronously, or acquires it if not cached.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="acquire">The function to acquire the item if it is not in the cache.</param>
        /// <returns>A task representing the asynchronous operation, containing the cached or acquired item.</returns>
        Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);

        /// <summary>
        /// Gets a cached item asynchronously, or acquires it if not cached.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="acquire">The function to acquire the item if it is not in the cache.</param>
        /// <returns>A task representing the asynchronous operation, containing the cached or acquired item.</returns>
        Task<T> GetAsync<T>(CacheKey key, Func<T> acquire);

        /// <summary>
        /// Gets a cached item, or acquires it if not cached.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="acquire">The function to acquire the item if it is not in the cache.</param>
        /// <returns>The cached or acquired item.</returns>
        T Get<T>(CacheKey key, Func<T> acquire);

        /// <summary>
        /// Removes a cached item asynchronously.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheKeyParameters">Additional parameters for the cache key.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters);

        /// <summary>
        /// Sets a cached item asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync(CacheKey key, object data);

        /// <summary>
        /// Removes cached items asynchronously by prefix.
        /// </summary>
        /// <param name="prefix">The cache key prefix.</param>
        /// <param name="prefixParameters">Additional parameters for the prefix.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

        /// <summary>
        /// Clears all cached items asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearAsync();

        /// <summary>
        /// Prepares a cache key with optional parameters.
        /// </summary>
        /// <param name="cacheKey">The base cache key.</param>
        /// <param name="cacheKeyParameters">Additional parameters for the cache key.</param>
        /// <returns>The prepared cache key.</returns>
        CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters);

        /// <summary>
        /// Prepares a cache key for default caching with optional parameters.
        /// </summary>
        /// <param name="cacheKey">The base cache key.</param>
        /// <param name="cacheKeyParameters">Additional parameters for the cache key.</param>
        /// <returns>The prepared cache key with default cache time.</returns>
        CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters);

        /// <summary>
        /// Prepares a cache key for short-term caching with optional parameters.
        /// </summary>
        /// <param name="cacheKey">The base cache key.</param>
        /// <param name="cacheKeyParameters">Additional parameters for the cache key.</param>
        /// <returns>The prepared cache key with short-term cache time.</returns>
        CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] cacheKeyParameters);
    }
}
