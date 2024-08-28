using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Json;
using CSharpFunctionalExtensions;
using IdentityModel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        }

        private static IConnectionMultiplexer InitializeRedis(string connectionString)
        {
            return ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString));
        }

        public Result<TUser, Error> ValidateCredentials(string username, string password)
        {
            return ExecuteAndCache(() => _ldapService.Login(username, password, null));
        }

        public Result<TUser, Error> ValidateCredentials(string username, string password, string domain)
        {
            return ExecuteAndCache(() => _ldapService.Login(username, password, domain));
        }

        public Result<TUser, Error> FindBySubjectId(string subjectId)
        {
            return FindAndCache(nameof(subjectId), subjectId,
                () => _ldapService.FindUser(subjectId.Replace("ldap_", "")));
        }

        public Result<TUser, Error> FindByUsername(string username)
        {
            return FindAndCache(nameof(username), username, () => _ldapService.FindUser(username));
        }

        public Result<TUser, Error> FindByEmail(string email)
        {
            return FindAndCache(nameof(email), email, () => _ldapService.FindUserByEmail(email));
        }

        public Result<List<string>, Error> GetUserAttributes(string attributeName, string attributeValue, string domain)
        {
            return _ldapService.GetUserAttributes(attributeName, attributeValue, domain);
        }

        public Result<TUser, Error> FindByExternalProvider(string provider, string userId)
        {
            var database = _redis.GetDatabase();
            var redisKey = $"IdentityServer/OpenId/provider/{provider}/userId/{userId}";
            var redisValue = database.StringGet(redisKey);

            if (redisValue.HasValue)
            {
                var key = redisValue.ToString();
                var userValue = database.StringGet(key);
                if (userValue.HasValue)
                {
                    return userValue.ToString().TryDeserializeObject<TUser>(_logger, _jsonSettings);
                }

                _logger.LogWarning($"The key {key} should not be existing or data is corrupted!");
            }

            return Result.Failure<TUser, Error>(new Error("USER_NOT_FOUND", "User not found in external provider"));
        }


        public Result<TUser, Error> AutoProvisionUser(string provider, string userId, List<Claim> claims)
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

            SetRedisData(user);

            return user;
        }

        private Result<TUser, Error> FindAndCache(string keyType, string key, Func<Result<TUser, Error>> fetchUser)
        {
            var redisKey = GetRedisKey(keyType, key);
            var cachedUser = GetUserFromRedis(redisKey);

            if (cachedUser != null)
            {
                return cachedUser;
            }

            var fetchResult = fetchUser();
            if (fetchResult.IsFailure)
            {
                return fetchResult;
            }

            SetRedisData(fetchResult.Value);

            return fetchResult;
        }

        private Result<TUser, Error> ExecuteAndCache(Func<Result<TUser, Error>> action)
        {
            try
            {
                var result = action();
                if (result.IsSuccess)
                {
                    SetRedisData(result.Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the action");
                return Result.Failure<TUser, Error>(new Error("INTERNAL_SERVER_ERROR", ex.Message));
            }
        }

        private TUser? GetUserFromRedis(string redisKey)
        {
            var redisValue = _redis.GetDatabase().StringGet(redisKey);

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

        private void SetRedisData(TUser user)
        {
            var database = _redis.GetDatabase();
            var userData = JsonConvert.SerializeObject(user, _jsonSettings);

            database.StringSet(GetRedisKey("subjectId", user.SubjectId), userData);
            database.StringSet(GetRedisKey("username", user.Username), user.SubjectId);
            database.StringSet(GetRedisKey("email", user.Email), user.SubjectId);
            database.StringSet(GetRedisKey("provider", $"{user.ProviderName}/userId/{user.ProviderSubjectId}"),
                user.SubjectId);
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