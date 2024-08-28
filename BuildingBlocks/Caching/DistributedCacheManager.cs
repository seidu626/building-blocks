using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.ComponentModel;
using BuildingBlocks.Configuration;
using BuildingBlocks.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace BuildingBlocks.Caching
{
    public class DistributedCacheManager : CacheKeyService, ILocker, IStaticCacheManager, IDisposable
    {
        private readonly IDistributedCache _distributedCache;
        private readonly PerRequestCache _perRequestCache;
        private readonly ILogger<DistributedCacheManager> _logger;
        private static readonly List<string> _keys = new List<string>();
        private static readonly AsyncLock _locker = new AsyncLock();

        public DistributedCacheManager(
            CacheConfig cacheConfig,
            IDistributedCache distributedCache,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DistributedCacheManager> logger)
            : base(cacheConfig)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _perRequestCache = new PerRequestCache(httpContextAccessor);
        }

        private DistributedCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };
        }

        private async Task<(bool isSet, T item)> TryGetItemAsync<T>(CacheKey key)
        {
            var stringAsync = await _distributedCache.GetStringAsync(key.Key);
            if (string.IsNullOrEmpty(stringAsync))
                return (false, default(T));

            var data = stringAsync.TryDeserializeObject<T>(_logger);
            _perRequestCache.Set(key.Key, data);
            return (true, data);
        }

        private (bool isSet, T item) TryGetItem<T>(CacheKey key)
        {
            var json = _distributedCache.GetString(key.Key);
            if (string.IsNullOrEmpty(json))
                return (false, default(T));

            var data = json.TryDeserializeObject<T>(_logger);
            _perRequestCache.Set(key.Key, data);
            return (true, data);
        }

        private void Set(CacheKey key, object data)
        {
            if (key?.CacheTime <= 0 || data == null)
                return;

            _distributedCache.SetString(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key));
            _perRequestCache.Set(key.Key, data);

            using (_locker.Lock())
            {
                _keys.Add(key.Key);
            }
        }

        public void Dispose()
        {
        }

        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            if (_perRequestCache.IsSet(key.Key))
                return _perRequestCache.Get(key.Key, () => default(T));

            if (key.CacheTime <= 0)
                return await acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);
            if (isSet)
                return item;

            var result = await acquire();
            if (result != null)
                await SetAsync(key, result);

            return result;
        }

        public async Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
        {
            if (_perRequestCache.IsSet(key.Key))
                return _perRequestCache.Get(key.Key, () => default(T));

            if (key.CacheTime <= 0)
                return acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);
            if (isSet)
                return item;

            var result = acquire();
            if (result != null)
                await SetAsync(key, result);

            return result;
        }

        public T Get<T>(CacheKey key, Func<T> acquire)
        {
            if (_perRequestCache.IsSet(key.Key))
                return _perRequestCache.Get(key.Key, () => default(T));

            if (key.CacheTime <= 0)
                return acquire();

            var (isSet, item) = TryGetItem<T>(key);
            if (isSet)
                return item;

            var data = acquire();
            if (data != null)
                Set(key, data);

            return data;
        }

        public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            cacheKey = PrepareKey(cacheKey, cacheKeyParameters);
            await _distributedCache.RemoveAsync(cacheKey.Key);
            _perRequestCache.Remove(cacheKey.Key);

            using (await _locker.LockAsync())
            {
                _keys.Remove(cacheKey.Key);
            }
        }

        public async Task SetAsync(CacheKey key, object data)
        {
            if (key?.CacheTime <= 0 || data == null)
                return;

            await _distributedCache.SetStringAsync(key.Key, JsonConvert.SerializeObject(data),
                PrepareEntryOptions(key));
            _perRequestCache.Set(key.Key, data);

            using (await _locker.LockAsync())
            {
                _keys.Add(key.Key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            prefix = PrepareKeyPrefix(prefix, prefixParameters);
            _perRequestCache.RemoveByPrefix(prefix);

            using (await _locker.LockAsync())
            {
                var keysToRemove = _keys
                    .Where(key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList();
                foreach (var key in keysToRemove)
                {
                    await _distributedCache.RemoveAsync(key);
                    _keys.Remove(key);
                }
            }
        }

        public async Task ClearAsync()
        {
            foreach (var key in _keys)
            {
                _perRequestCache.Remove(key);
            }

            using (await _locker.LockAsync())
            {
                foreach (var key in _keys)
                {
                    await _distributedCache.RemoveAsync(key);
                }

                _keys.Clear();
            }
        }

        public bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action)
        {
            if (!string.IsNullOrEmpty(_distributedCache.GetString(resource)))
                return false;

            try
            {
                _distributedCache.SetString(resource, resource, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime
                });

                action();
                return true;
            }
            finally
            {
                _distributedCache.Remove(resource);
            }
        }

        protected class PerRequestCache
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

            public PerRequestCache(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            protected virtual IDictionary<object, object> GetItems()
            {
                return _httpContextAccessor.HttpContext?.Items;
            }

            public virtual T Get<T>(string key, Func<T> acquire)
            {
                IDictionary<object, object> items;
                using (new ReaderWriteLockDisposable(_lockSlim, ReaderWriteLockType.Read))
                {
                    items = GetItems();
                    if (items == null)
                        return acquire();

                    if (items.ContainsKey(key))
                        return (T)items[key];
                }

                var result = acquire();
                using (new ReaderWriteLockDisposable(_lockSlim))
                {
                    items[key] = result;
                }

                return result;
            }

            public virtual void Set(string key, object data)
            {
                if (data == null)
                    return;

                using (new ReaderWriteLockDisposable(_lockSlim))
                {
                    var items = GetItems();
                    if (items == null)
                        return;

                    items[key] = data;
                }
            }

            public virtual bool IsSet(string key)
            {
                using (new ReaderWriteLockDisposable(_lockSlim, ReaderWriteLockType.Read))
                {
                    return GetItems()?.ContainsKey(key) == true;
                }
            }

            public virtual void Remove(string key)
            {
                using (new ReaderWriteLockDisposable(_lockSlim))
                {
                    GetItems()?.Remove(key);
                }
            }

            public virtual void RemoveByPrefix(string prefix)
            {
                using (new ReaderWriteLockDisposable(_lockSlim, ReaderWriteLockType.UpgradeableRead))
                {
                    var items = GetItems();
                    if (items == null)
                        return;

                    var regex = new Regex(prefix,
                        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
                    var keysToRemove = items.Keys.Cast<string>().Where(key => regex.IsMatch(key ?? string.Empty))
                        .ToList();

                    if (keysToRemove.Any())
                    {
                        using (new ReaderWriteLockDisposable(_lockSlim))
                        {
                            foreach (var key in keysToRemove)
                            {
                                items.Remove(key);
                            }
                        }
                    }
                }
            }
        }
    }
}