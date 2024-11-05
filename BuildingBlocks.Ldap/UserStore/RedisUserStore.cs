using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Common.AOP;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Json;
using CSharpFunctionalExtensions;
using IdentityModel;
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
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncFallbackPolicy _fallbackPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        // Fallback in-memory storage in case Redis fails
        private readonly ConcurrentDictionary<string, TUser> _fallbackCache = new ConcurrentDictionary<string, TUser>();

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
        }

        private static IConnectionMultiplexer InitializeRedis(string connectionString)
        {
            return ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString));
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password)
        {
            return await ExecuteAndCacheAsync(() => _ldapService.Login(username, password, null), "username", username);
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password,
            string? domain)
        {
            return await ExecuteAndCacheAsync(() => _ldapService.Login(username, password, domain), "username",
                username);
        }

        public async Task<Result<TUser, Error>> FindBySubjectIdAsync(string subjectId)
        {
            return await FindAndCacheAsync(nameof(subjectId), subjectId,
                () => _ldapService.FindUser(subjectId.Replace("ldap_", "")));
        }

        public async Task<Result<TUser, Error>> FindByUsernameAsync(string username)
        {
            return await FindAndCacheAsync(nameof(username), username, () => _ldapService.FindUser(username));
        }

        public async Task<Result<TUser, Error>> FindByEmailAsync(string email)
        {
            return await FindAndCacheAsync(nameof(email), email, () => _ldapService.FindUserByEmail(email));
        }

        public async Task<Result<TUser, Error>> FindByPhoneAsync(string phone)
        {
            return await FindAndCacheAsync(nameof(phone), phone, () => _ldapService.FindUserByPhone(phone));
        }

        public async Task<Result<List<string>, Error>> GetUserAttributesAsync(string attributeName,
            string attributeValue, string domain)
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

        public async Task<Result<TUser, Error>> AutoProvisionUserAsync(string? provider, string userId,
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
        private async Task<Result<TUser, Error>> FindAndCacheAsync(string keyType, string key,
            Func<Task<Result<TUser, Error>>> fetchUser)
        {
            var redisKey = GetRedisKey(keyType, key);

            // Attempt to get the user from Redis
            var cachedUser = await _retryPolicy.ExecuteAsync(async () => await GetUserFromRedisAsync(redisKey));

            if (cachedUser != null)
            {
                return cachedUser;
            }

            // If Redis fails or no user is found, check fallback cache
            if (_fallbackCache.TryGetValue(redisKey, out var fallbackUser))
            {
                return fallbackUser;
            }

            // Fetch the user from the source (LDAP service)
            var fetchResult = await fetchUser();
            if (fetchResult.IsFailure)
            {
                return fetchResult;
            }

            // Save to both Redis and in-memory fallback cache
            await SetRedisDataAsync(fetchResult.Value);
            _fallbackCache[redisKey] = fetchResult.Value;

            return fetchResult;
        }
        
        [Time] [Timeout(1200)]
        private async Task<Result<TUser, Error>> ExecuteAndCacheAsync(Func<Task<Result<TUser, Error>>> action,
            string keyType, string key)
        {
            try
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var result = await action();
                    if (!result.IsSuccess) return result;
                    await SetRedisDataAsync(result.Value);
                    _fallbackCache[GetRedisKey(keyType, key)] = result.Value;

                    return result;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the Redis operation.");
                return Result.Failure<TUser, Error>(new Error("INTERNAL_SERVER_ERROR", ex.Message, ex));
            }
        }

        [Time] [Timeout(1200)]
        private async Task<TUser?> GetUserFromRedisAsync(string redisKey)
        {
            var redisValue = await _redis.GetDatabase().StringGetAsync(redisKey);
            if (redisValue.HasValue)
            {
                return redisValue.ToString().TryDeserializeObject<TUser>(_logger, _jsonSettings);
            }

            return default;
        }

        private static string GetRedisKey(string type, string key)
        {
            return $"IdentityServer/OpenId/{type}/{key}";
        }

        [Time] [Timeout(1200)]
        private async Task SetRedisDataAsync(TUser user)
        {
            var database = _redis.GetDatabase();
            var userData = JsonConvert.SerializeObject(user, _jsonSettings);

            var tasks = new List<Task>
            {
                database.StringSetAsync(GetRedisKey("subjectId", user.SubjectId), userData, _dataExpireIn),
                database.StringSetAsync(GetRedisKey("username", user.Username), user.SubjectId, _dataExpireIn),
                database.StringSetAsync(GetRedisKey("email", user.Email), user.SubjectId, _dataExpireIn),
                database.StringSetAsync(GetRedisKey("provider", $"{user.ProviderName}/userId/{user.ProviderSubjectId}"),
                    user.SubjectId, _dataExpireIn)
            };

            await Task.WhenAll(tasks);
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