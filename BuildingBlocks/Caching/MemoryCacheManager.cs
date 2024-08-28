using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Caching
{
    public class MemoryCacheManager : CacheKeyService, ILocker, IStaticCacheManager, IDisposable
    {
        private bool _disposed;
        private readonly IMemoryCache _memoryCache;
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new ConcurrentDictionary<string, CancellationTokenSource>();
        private static CancellationTokenSource _clearToken = new CancellationTokenSource();

        public MemoryCacheManager(CacheConfig cacheConfig, IMemoryCache memoryCache)
            : base(cacheConfig)
        {
            _memoryCache = memoryCache;
        }

        private MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };

            cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));

            foreach (var prefix in key.Prefixes)
            {
                var prefixTokenSource = _prefixes.GetOrAdd(prefix, new CancellationTokenSource());
                cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(prefixTokenSource.Token));
            }

            return cacheEntryOptions;
        }

        private void Remove(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var key = PrepareKey(cacheKey, cacheKeyParameters);
            _memoryCache.Remove(key.Key);
        }

        private void Set(CacheKey key, object data)
        {
            if (key?.CacheTime > 0 && data != null)
            {
                _memoryCache.Set(key.Key, data, PrepareEntryOptions(key));
            }
        }

        public Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            Remove(cacheKey, cacheKeyParameters);
            return Task.CompletedTask;
        }

        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            if (key?.CacheTime <= 0)
            {
                return await acquire();
            }

            if (_memoryCache.TryGetValue(key.Key, out T result))
            {
                return result;
            }

            result = await acquire();
            if (result != null)
            {
                await SetAsync(key, result);
            }

            return result;
        }

        public async Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
        {
            if (key?.CacheTime <= 0)
            {
                return acquire();
            }

            var result = _memoryCache.GetOrCreate(key.Key, entry =>
            {
                entry.SetOptions(PrepareEntryOptions(key));
                return acquire();
            });

            if (result == null)
            {
                await RemoveAsync(key);
            }

            return result;
        }

        public T Get<T>(CacheKey key, Func<T> acquire)
        {
            if (key?.CacheTime <= 0)
            {
                return acquire();
            }

            if (_memoryCache.TryGetValue(key.Key, out T data))
            {
                return data;
            }

            data = acquire();
            if (data != null)
            {
                Set(key, data);
            }

            return data;
        }

        public Task SetAsync(CacheKey key, object data)
        {
            Set(key, data);
            return Task.CompletedTask;
        }

        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            if (_memoryCache.TryGetValue(key, out _))
            {
                return false;
            }

            try
            {
                _memoryCache.Set(key, key, expirationTime);
                action();
                return true;
            }
            finally
            {
                _memoryCache.Remove(key);
            }
        }

        public Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            var formattedPrefix = PrepareKeyPrefix(prefix, prefixParameters);
            if (_prefixes.TryRemove(formattedPrefix, out var tokenSource))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }

            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();
            _clearToken = new CancellationTokenSource();

            foreach (var prefix in _prefixes.Keys.ToList())
            {
                if (_prefixes.TryRemove(prefix, out var tokenSource))
                {
                    tokenSource.Dispose();
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _memoryCache.Dispose();
            }

            _disposed = true;
        }
    }
}
