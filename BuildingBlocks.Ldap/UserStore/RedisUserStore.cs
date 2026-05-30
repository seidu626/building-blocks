using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Common;
using BuildingBlocks.Common.AOP;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Json;
using CSharpFunctionalExtensions;
using Duende.IdentityModel;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using StackExchange.Redis;

namespace BuildingBlocks.Ldap.UserStore
{
    public class RedisUserStore<TUser> : ILdapUserStore<TUser> where TUser : IAppUser, new()
    {
        private readonly ILdapService<TUser> _ldapService;
        private readonly ILogger<RedisUserStore<TUser>> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _dataExpireIn;
        private readonly TimeSpan _fallbackCacheTtl;
        private readonly TimeSpan _fallbackCacheSweepInterval = TimeSpan.FromMinutes(2);
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncFallbackPolicy _fallbackPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        // Fallback in-memory storage in case Redis fails
        private readonly ConcurrentDictionary<string, CachedDirectoryUser> _fallbackCache =
            new ConcurrentDictionary<string, CachedDirectoryUser>();

        private DateTimeOffset _nextFallbackCacheSweepUtc = DateTimeOffset.UtcNow;
        private readonly object _fallbackCacheSweepLock = new();

        private sealed class CachedDirectoryUser
        {
            public bool IsFound { get; }
            public TUser? User { get; }
            public string? ErrorCode { get; }
            public string? ErrorMessage { get; }
            public DateTimeOffset CachedAtUtc { get; }

            public CachedDirectoryUser(TUser user, DateTimeOffset cachedAtUtc)
            {
                IsFound = true;
                User = user;
                CachedAtUtc = cachedAtUtc;
            }

            public CachedDirectoryUser(Error error, DateTimeOffset cachedAtUtc)
            {
                IsFound = false;
                ErrorCode = error?.Code;
                ErrorMessage = error?.FriendlyMessage;
                User = default;
                CachedAtUtc = cachedAtUtc;
            }
        }

        private sealed class RedisDirectoryLookupCacheEntry
        {
            public bool IsFound { get; set; }
            public string? LookupKey { get; set; }
            public string? ErrorCode { get; set; }
            public string? ErrorMessage { get; set; }
        }

        private sealed class CachedLookupResult
        {
            public bool IsHit { get; set; }
            public bool IsFound { get; set; }
            public TUser? User { get; set; }
            public Error? Error { get; set; }
        }

        public RedisUserStore(ILdapService<TUser> ldapService, ExtensionConfig config,
            ILogger<RedisUserStore<TUser>> logger)
        {
            _ldapService = ldapService ?? throw new ArgumentNullException(nameof(ldapService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _redis = !string.IsNullOrEmpty(config.Redis)
                ? InitializeRedis(config.Redis)
                : throw new RedisConnectionException(ConnectionFailureType.UnableToConnect,
                    "Missing Redis configuration in Startup.cs");

            _dataExpireIn = TimeSpan.FromSeconds(config.RefreshClaimsInSeconds);
            _jsonSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ClaimConverter() },
                Formatting = Formatting.Indented
            };

            // Polly retry and circuit breaker policies
            _retryPolicy = Policy
                .Handle<RedisTimeoutException>()
                .Or<RedisException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} for Redis operation due to: {exception.Message}");
                    });


            _fallbackPolicy = Policy
                .Handle<RedisTimeoutException>()
                .Or<RedisException>()
                .Or<Exception>()
                .FallbackAsync((action) =>
                {
                    _logger.LogWarning("Redis operation failed, falling back to in-memory cache.");
                    return Task.CompletedTask;
                });


            _circuitBreakerPolicy = Policy
                .Handle<RedisTimeoutException>()
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1),
                    (exception, timespan) =>
                    {
                        _logger.LogWarning("Circuit breaker triggered due to Redis failures.");
                    },
                    () => { _logger.LogInformation("Circuit breaker reset."); });

            _fallbackCacheTtl = _dataExpireIn > TimeSpan.Zero
                ? _dataExpireIn
                : TimeSpan.FromMinutes(5);
        }

        private static IConnectionMultiplexer InitializeRedis(string connectionString)
        {
            return ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString));
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password,
            IdentifierType identifierType)
        {
            return await ExecuteAndCacheAsync(
                () => _ldapService.Login(username, password, identifierType, null),
                BuildCredentialsLookupType(identifierType),
                BuildCredentialsLookupKey(username, null));
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password,
            IdentifierType identifierType,
            string domain)
        {
            return await ExecuteAndCacheAsync(
                () => _ldapService.Login(username, password, identifierType, domain),
                BuildCredentialsLookupType(identifierType),
                BuildCredentialsLookupKey(username, domain));
        }

        public async Task<Result<TUser, Error>> FindBySubjectIdAsync(string? subjectId)
        {
            return await FindAndCacheAsync(nameof(subjectId), subjectId,
                () => _ldapService.FindUser(subjectId?.Replace("ldap_", "")));
        }

        public async Task<Result<TUser, Error>> FindByUsernameAsync(string? username)
        {
            return await FindAndCacheAsync(nameof(username), username, () => _ldapService.FindUser(username));
        }

        public async Task<Result<TUser, Error>> FindByEmailAsync(string? email)
        {
            return await FindAndCacheAsync(nameof(email), email, () => _ldapService.FindUserByEmail(email));
        }

        public async Task<Result<TUser, Error>> FindByPhoneAsync(string? phone)
        {
            return await FindAndCacheAsync(nameof(phone), phone, () => _ldapService.FindUserByPhone(phone));
        }

        public async Task<Result<List<string>, Error>> GetUserAttributesAsync(string attributeName,
            string? attributeValue, string domain)
        {
            return await _ldapService.GetUserAttributes(attributeName, attributeValue, domain);
        }

        public async Task<Result<TUser, Error>> FindByExternalProviderAsync(string? provider, string userId)
        {
            return await _fallbackPolicy.WrapAsync(_retryPolicy).ExecuteAsync(async () =>
            {
                var database = _redis.GetDatabase();
                var redisKey = $"IdentityServer/OpenId/provider/{provider}/userId/{userId}";
                var redisValue = await database.StringGetAsync(redisKey);

                if (!redisValue.HasValue)
                    return Result.Failure<TUser, Error>(new Error("USER_NOT_FOUND",
                        "User not found in external provider"));
                var key = redisValue.ToString();
                var userValue = await database.StringGetAsync(key);
                if (userValue.HasValue)
                {
                    return userValue.ToString().TryDeserializeObject<TUser>(_logger, _jsonSettings);
                }

                _logger.LogWarning($"The key {key} should not exist or data is corrupted!");

                return Result.Failure<TUser, Error>(new Error("USER_NOT_FOUND", "User not found in external provider"));
            });
        }

        public async Task<Result<TUser, Error>> AutoProvisionUserAsync(string? provider, string? userId,
            List<Claim> claims)
        {
            var filteredClaims = FilterClaims(claims);
            var uniqueId = CryptoRandom.CreateUniqueId();
            var userName = filteredClaims.FirstOrDefault(c => c.Type == "name")?.Value ?? uniqueId;

            var user = new TUser
            {
                SubjectId = uniqueId,
                Username = userName,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = filteredClaims
            };

            await SetRedisDataAsync(user);

            return user;
        }

        [Time] [Timeout(1200)]
        private async Task<Result<TUser, Error>> FindAndCacheAsync(string keyType, string? key,
            Func<Task<Result<TUser, Error>>> fetchUser)
        {
            var redisKey = GetRedisKey(keyType, key);
            PurgeExpiredFallbackEntries();

            if (IsLookupKeyInvalid(key))
            {
                return await fetchUser();
            }

            // Attempt to get the user from Redis
            var redisLookup = await GetCachedLookupFromRedisAsync(redisKey);
            if (redisLookup != null && redisLookup.IsHit)
            {
                return redisLookup.IsFound
                    ? Result.Success<TUser, Error>(redisLookup.User!)
                    : Result.Failure<TUser, Error>(redisLookup.Error!);
            }

            // If Redis fails or no user is found, check fallback cache
            if (TryGetFallbackLookup(redisKey, out var fallbackUser, out var fallbackError))
            {
                return fallbackUser != null
                    ? Result.Success<TUser, Error>(fallbackUser)
                    : Result.Failure<TUser, Error>(fallbackError!);
            }

            // Fetch the user from the source (LDAP service)
            var fetchResult = await fetchUser();
            if (fetchResult.IsFailure)
            {
                await CacheLookupResultAsync(fetchResult, redisKey, shouldCacheFailure: IsCacheableDirectoryLookupFailure(fetchResult.Error));
                return fetchResult;
            }

            // Save to both Redis and in-memory fallback cache
            await CacheLookupResultAsync(fetchResult, redisKey);

            return fetchResult;
        }
        
        [Time] [Timeout(1200)]
        private async Task<Result<TUser, Error>> ExecuteAndCacheAsync(Func<Task<Result<TUser, Error>>> action,
            string keyType, string? key)
        {
            try
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var result = await action();
                    await CacheLookupResultAsync(result, GetRedisKey(keyType, key),
                        shouldCacheFailure: IsCacheableDirectoryLookupFailure(result.Error));

                    return result;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the Redis operation.");
                return Result.Failure<TUser, Error>(new Error("INTERNAL_SERVER_ERROR", ex.Message, ex));
            }
        }

        private async Task<CachedLookupResult?> GetCachedLookupFromRedisAsync(string redisKey)
        {
            try
            {
                var redisValue = await _retryPolicy.ExecuteAsync(async () => await GetCachedLookupValueFromRedisAsync(redisKey));
                if (redisValue == null)
                {
                    return null;
                }

                return redisValue;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogWarning(ex, "Redis timeout while reading user cache key {RedisKey}", redisKey);
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis error while reading user cache key {RedisKey}", redisKey);
            }

            return default;
        }

        private async Task<CachedLookupResult?> GetCachedLookupValueFromRedisAsync(string redisKey)
        {
            var redisValue = await _redis.GetDatabase().StringGetAsync(redisKey);
            if (!redisValue.HasValue)
            {
                return null;
            }

            var rawValue = redisValue.ToString();
            var cachedUser = rawValue.TryDeserializeObject<TUser>(_logger, _jsonSettings);
            if (cachedUser != null)
            {
                return new CachedLookupResult { IsHit = true, IsFound = true, User = cachedUser };
            }

            var cacheEntry = rawValue.TryDeserializeObject<RedisDirectoryLookupCacheEntry>(_logger, _jsonSettings);
            if (cacheEntry != null)
            {
                return cacheEntry.IsFound
                    ? null
                    : new CachedLookupResult
                    {
                        IsHit = true,
                        IsFound = false,
                        Error = new Error(cacheEntry.ErrorCode ?? "404", cacheEntry.ErrorMessage ?? "User not found.")
                    };
            }

            // Fallback for provider key mapping: stored value may be the subject ID
            var userBySubjectId = await GetUserFromRedisBySubjectIdAsync(rawValue);
            if (userBySubjectId != null)
            {
                return new CachedLookupResult { IsHit = true, IsFound = true, User = userBySubjectId };
            }

            return null;
        }

        private async Task CacheLookupResultAsync(Result<TUser, Error> lookupResult, string redisKey,
            bool shouldCacheFailure = false)
        {
            if (lookupResult.IsSuccess)
            {
                var user = lookupResult.Value;
                if (user == null)
                {
                    return;
                }

                SetFallbackCache(redisKey, user);
                await TrySetRedisDataAsync(user);
                return;
            }

            if (!shouldCacheFailure)
            {
                return;
            }

            SetFallbackCache(redisKey, lookupResult.Error!);
            await TrySetRedisFailureAsync(redisKey, lookupResult.Error!);
        }

        private void SetFallbackCache(string redisKey, TUser user)
        {
            _fallbackCache[redisKey] = new CachedDirectoryUser(user, DateTimeOffset.UtcNow);
        }

        private void SetFallbackCache(string redisKey, Error error)
        {
            _fallbackCache[redisKey] = new CachedDirectoryUser(error, DateTimeOffset.UtcNow);
        }

        private bool TryGetFallbackLookup(string redisKey, out TUser? user, out Error? error)
        {
            _fallbackCache.TryGetValue(redisKey, out var fallbackEntry);

            if (fallbackEntry != null && !IsFallbackEntryExpired(fallbackEntry))
            {
                user = fallbackEntry.User;
                error = fallbackEntry.ErrorCode == null
                    ? null
                    : new Error(fallbackEntry.ErrorCode,
                        fallbackEntry.ErrorMessage ?? "User not found in directory cache.");
                return true;
            }

            if (fallbackEntry != null)
            {
                _fallbackCache.TryRemove(redisKey, out _);
            }

            user = default;
            error = null;
            return false;
        }

        private static bool IsLookupKeyInvalid(string key)
        {
            return string.IsNullOrWhiteSpace(key);
        }

        private static bool IsCacheableDirectoryLookupFailure(Error error)
        {
            if (error == null)
            {
                return false;
            }

            return string.Equals(error.Code, "404", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(error.Code, "USER_NOT_FOUND", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(error.Code, "UserNotFound", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsFallbackEntryExpired(CachedDirectoryUser cachedDirectoryUser)
        {
            return DateTimeOffset.UtcNow - cachedDirectoryUser.CachedAtUtc > _fallbackCacheTtl;
        }

        private void PurgeExpiredFallbackEntries()
        {
            var now = DateTimeOffset.UtcNow;
            if (now < _nextFallbackCacheSweepUtc)
            {
                return;
            }

            lock (_fallbackCacheSweepLock)
            {
                if (now < _nextFallbackCacheSweepUtc)
                {
                    return;
                }

                foreach (var key in _fallbackCache.Keys)
                {
                    if (_fallbackCache.TryGetValue(key, out var entry)
                        && IsFallbackEntryExpired(entry))
                    {
                        _fallbackCache.TryRemove(key, out _);
                    }
                }

                _nextFallbackCacheSweepUtc = now + _fallbackCacheSweepInterval;
            }
        }

        private async Task TrySetRedisDataAsync(TUser user)
        {
            try
            {
                await SetRedisDataAsync(user);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogWarning(ex, "Redis timeout while writing user cache entry.");
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis error while writing user cache entry.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unexpected error while writing user cache entry.");
            }
        }

        private async Task TrySetRedisDataAsync(string redisKey, Error error)
        {
            try
            {
                await SetRedisLookupDataAsync(redisKey, error);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogWarning(ex, "Redis timeout while writing lookup cache entry for key {RedisKey}.", redisKey);
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis error while writing lookup cache entry for key {RedisKey}.", redisKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unexpected error while writing lookup cache entry for key {RedisKey}.", redisKey);
            }
        }

        private async Task SetRedisLookupDataAsync(string redisKey, Error error)
        {
            var database = _redis.GetDatabase();
            var cacheEntry = new RedisDirectoryLookupCacheEntry
            {
                IsFound = false,
                LookupKey = redisKey,
                ErrorCode = error?.Code,
                ErrorMessage = error?.FriendlyMessage
            };

            var lookupData = JsonConvert.SerializeObject(cacheEntry, _jsonSettings);
            await database.StringSetAsync(redisKey, lookupData, _dataExpireIn);
        }

        [Time] [Timeout(1200)]
        private async Task<TUser?> GetUserFromRedisAsync(string redisKey)
        {
            var redisValue = await _redis.GetDatabase().StringGetAsync(redisKey);
            if (redisValue.HasValue)
            {
                var rawValue = redisValue.ToString();
                var cachedUser = rawValue.TryDeserializeObject<TUser>(_logger, _jsonSettings);

                if (cachedUser != null)
                {
                    return cachedUser;
                }

                return await GetUserFromRedisBySubjectIdAsync(rawValue);
            }

            return default;
        }

        private static string GetRedisKey(string type, string? key)
        {
            return $"IdentityServer/OpenId/{type}/{key}";
        }

        private static string BuildCredentialsLookupType(IdentifierType identifierType)
        {
            return identifierType switch
            {
                IdentifierType.Email => "email",
                IdentifierType.Phone => "phone",
                _ => "username"
            };
        }

        private static string BuildCredentialsLookupKey(string? identifier, string? domain)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(domain))
            {
                return identifier;
            }

            return $"{domain}:{identifier}";
        }

        [Time] [Timeout(1200)]
        private async Task SetRedisDataAsync(TUser user)
        {
            if (user == null)
            {
                return;
            }

            var database = _redis.GetDatabase();
            var userData = JsonConvert.SerializeObject(user, _jsonSettings);
            var cacheTasks = new List<Task>
            {
                database.StringSetAsync(GetRedisKey("subjectId", user.SubjectId), userData, _dataExpireIn)
            };

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                cacheTasks.Add(database.StringSetAsync(GetRedisKey("username", user.Username), userData,
                    _dataExpireIn));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                cacheTasks.Add(database.StringSetAsync(GetRedisKey("email", user.Email), userData,
                    _dataExpireIn));
            }

            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                cacheTasks.Add(database.StringSetAsync(GetRedisKey("phone", user.Mobile), userData,
                    _dataExpireIn));
            }

            if (!string.IsNullOrWhiteSpace(user.ProviderName) &&
                !string.IsNullOrWhiteSpace(user.ProviderSubjectId))
            {
                cacheTasks.Add(database.StringSetAsync(GetRedisKey("provider",
                    $"{user.ProviderName}/userId/{user.ProviderSubjectId}"),
                    user.SubjectId, _dataExpireIn));
            }

            await Task.WhenAll(cacheTasks);
        }

        [Time] [Timeout(1200)]
        private async Task<TUser?> GetUserFromRedisBySubjectIdAsync(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                return default;
            }

            var subjectCacheKey = GetRedisKey("subjectId", subjectId);
            var subjectCacheValue = await _redis.GetDatabase().StringGetAsync(subjectCacheKey);
            if (!subjectCacheValue.HasValue)
            {
                return default;
            }

            return subjectCacheValue.ToString().TryDeserializeObject<TUser>(_logger, _jsonSettings);
        }

        private List<Claim> FilterClaims(List<Claim> claims)
        {
            return claims.Select(claim =>
            {
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                {
                    return new Claim("name", claim.Value);
                }

                if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.TryGetValue(claim.Type, out var mappedType))
                {
                    return new Claim(mappedType, claim.Value);
                }

                return claim;
            }).ToList();
        }
    }
}
