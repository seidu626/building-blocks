using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BuildingBlocks.Caching
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheProvider> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private string _defaultCachedKey = "__DEFAULT_CACHE__";

        public CacheProvider(IDistributedCache cache, ILogger<CacheProvider> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _retryPolicy = Policy
                .Handle<TimeoutException>()
                .Or<OperationCanceledException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2.0, attempt)),
                    (exception, timespan, attempt, context) =>
                        _logger.LogError(exception, "Retry {RetryAttempt} for key {CacheKey} due to {ExceptionMessage}",
                            attempt, context["CacheKey"], exception.Message));
        }

        public string SetDefaultCachedKey(string key)
        {
            _defaultCachedKey = key;
            return key;
        }

        public async Task SetCachedKeysAsync(string data, string key, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async (ctx, ct) =>
            {
                var stringList = await GetAsync<List<string>>(ctx["CacheKey"].ToString()!, cancellationToken) ??
                                 new List<string>();
                if (!stringList.Contains(data))
                {
                    stringList.Add(data);
                    var serializedData = JsonSerializer.Serialize(stringList);
                    await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
                }
            }, new Context { ["CacheKey"] = key }, cancellationToken);
        }

        public async Task<T> GetAsync<T>(string key, DistributedCacheEntryOptions options, SemaphoreSlim semaphore,
            Func<Task<T>> asyncFunc, CancellationToken cancellationToken) where T : class
        {
            var record = await GetAsync<T>(key, cancellationToken);
            if (record != null)
                return record;

            try
            {
                await semaphore.WaitAsync(cancellationToken);

                // Fetching again after acquiring the semaphore to ensure consistency
                record = await GetAsync<T>(key, cancellationToken);
                if (record != null)
                    return record;

                record = await asyncFunc();
                await SetAsync(key, record, options, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }

            return record;
        }

        public async Task<T> GetAsync<T>(string key, DistributedCacheEntryOptions options, SemaphoreSlim semaphore,
            Func<T> func, CancellationToken cancellationToken) where T : class
        {
            var record = await GetAsync<T>(key, cancellationToken);
            if (record != null)
                return record;

            try
            {
                await semaphore.WaitAsync(cancellationToken);

                record = await GetAsync<T>(key, cancellationToken);
                if (record != null)
                    return record;

                record = func();
                await SetAsync(key, record, options, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }

            return record;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class
        {
            return await _retryPolicy.ExecuteAsync(async (ctx, ct) =>
            {
                var cachedString = await _cache.GetStringAsync(ctx["CacheKey"].ToString()!, cancellationToken);
                return string.IsNullOrWhiteSpace(cachedString) ? default : JsonSerializer.Deserialize<T>(cachedString);
            }, new Context { ["CacheKey"] = key }, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken) where T : class
        {
            var serializedValue = JsonSerializer.Serialize(value);

            await _retryPolicy.ExecuteAsync(async (ctx, ct) =>
            {
                // Ensure the correct options are being passed to the cache set operation
                await _cache.SetStringAsync(ctx["CacheKey"].ToString()!, serializedValue, options, ct);

                // Optionally, you can track this key if needed (based on your implementation)
                await SetCachedKeysAsync(ctx["CacheKey"].ToString()!, _defaultCachedKey, options, ct);
            }, new Context { ["CacheKey"] = key }, cancellationToken);
        }

        public async Task ClearAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<List<string>> FlushAsync(CancellationToken cancellationToken)
        {
            var records = await GetAsync<List<string>>(_defaultCachedKey, cancellationToken);
            if (records == null)
                return null;

            foreach (var key in records)
            {
                await ClearAsync(key);
            }

            await ClearAsync(_defaultCachedKey);
            return records;
        }
    }
}