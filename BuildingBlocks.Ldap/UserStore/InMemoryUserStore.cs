using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using BuildingBlocks.Common;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Exceptions;
using CSharpFunctionalExtensions;
using Duende.IdentityModel;

namespace BuildingBlocks.Ldap.UserStore
{
    public class InMemoryUserStore<TUser> : ILdapUserStore<TUser> where TUser : IAppUser, new()
    {
        private readonly ILdapService<TUser> _authenticationService;

        // Cache for provider/subject mapping (legacy fast-path for auto-provisioned users)
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, TUser>> _users =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, TUser>>();

        // Lookup cache with both success and failure envelopes
        private readonly ConcurrentDictionary<string, CachedDirectoryUser> _lookupCache =
            new ConcurrentDictionary<string, CachedDirectoryUser>();

        private readonly TimeSpan _fallbackCacheTtl = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _fallbackCacheSweepInterval = TimeSpan.FromMinutes(2);
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
                CachedAtUtc = cachedAtUtc;
            }
        }

        public InMemoryUserStore(ILdapService<TUser> authenticationService)
        {
            _authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password,
            IdentifierType identifierType)
        {
            return await ValidateCredentialsAsync(username, password, identifierType, null);
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password,
            IdentifierType identifierType,
            string domain)
        {
            var cacheKey = BuildLookupKey(identifierType.ToString(), BuildCredentialsLookupKey(username, domain));
            return await FindAndCacheAsync(cacheKey, () =>
                _authenticationService.Login(username, password, identifierType, domain));
        }

        public async Task<Result<TUser, Error>> FindBySubjectIdAsync(string? subjectId)
        {
            var normalizedSubjectId = subjectId?.Replace("ldap_", "");
            return await FindByKeyWithProviderFallbackAsync("subjectId", normalizedSubjectId,
                () => _authenticationService.FindUser(normalizedSubjectId));
        }

        public async Task<Result<TUser, Error>> FindByUsernameAsync(string? username)
        {
            var normalizedUsername = username?.Replace("ldap_", "");
            return await FindByKeyWithProviderFallbackAsync("username", normalizedUsername,
                () => _authenticationService.FindUser(normalizedUsername));
        }

        public async Task<Result<TUser, Error>> FindByEmailAsync(string? email)
        {
            var normalizedEmail = email?.Replace("ldap_", "");
            return await FindByKeyWithProviderFallbackAsync("email", normalizedEmail,
                () => _authenticationService.FindUserByEmail(normalizedEmail));
        }

        public async Task<Result<TUser, Error>> FindByPhoneAsync(string? phone)
        {
            var normalizedPhone = phone?.Replace("ldap_", "");
            return await FindByKeyWithProviderFallbackAsync("phone", normalizedPhone,
                () => _authenticationService.FindUserByPhone(normalizedPhone));
        }

        public async Task<Result<List<string>, Error>> GetUserAttributesAsync(string attributeName,
            string? attributeValue, string domain)
        {
            return await _authenticationService.GetUserAttributes(attributeName, attributeValue, domain);
        }

        public async Task<Result<TUser, Error>> FindByExternalProviderAsync(string? provider, string userId)
        {
            var providerKey = provider ?? string.Empty;
            if (_users.TryGetValue(providerKey, out var providerUsers) && providerUsers.TryGetValue(userId, out var user))
            {
                return await Task.FromResult(Result.Success<TUser, Error>(user));
            }

            return await Task.FromResult(
                Result.Failure<TUser, Error>(new Error("404", "User not found in external provider")));
        }

        public Task<Result<TUser, Error>> AutoProvisionUserAsync(string? provider, string? userId,
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

            CacheProviderUser(user);
            CacheLookupCache(BuildLookupKey("subjectId", user.SubjectId), user);
            CacheLookupCache(BuildLookupKey("username", user.Username), user);
            CacheLookupCache(BuildLookupKey("email", user.Email), user);
            CacheLookupCache(BuildLookupKey("phone", user.Mobile), user);

            return Task.FromResult<Result<TUser, Error>>(user);
        }

        private async Task<Result<TUser, Error>> PerformAuthenticationAsync(
            Func<Task<Result<TUser, Error>>> authenticationFunc,
            string cacheKey)
        {
            try
            {
                return await FindAndCacheAsync(cacheKey, authenticationFunc);
            }
            catch (LoginFailedException ex)
            {
                return Result.Failure<TUser, Error>(new Error("404", ex.Message, ex));
            }
        }

        private async Task<Result<TUser, Error>> FindByKeyWithProviderFallbackAsync(string keyType, string? key,
            Func<Task<Result<TUser, Error>>> fetchUser)
        {
            var cachedProviderUser = TryGetProviderCachedUser(u => keyType switch
            {
                "subjectId" => string.Equals(u.SubjectId, key, StringComparison.OrdinalIgnoreCase),
                "username" => string.Equals(u.Username, key, StringComparison.OrdinalIgnoreCase),
                "email" => string.Equals(u.Email, key, StringComparison.OrdinalIgnoreCase),
                "phone" => string.Equals(u.Mobile, key, StringComparison.OrdinalIgnoreCase),
                _ => false
            });

            if (cachedProviderUser != null)
            {
                return cachedProviderUser;
            }

            return await FindAndCacheAsync(BuildLookupKey(keyType, key), fetchUser);
        }

        private async Task<Result<TUser, Error>> FindAndCacheAsync(string cacheKey,
            Func<Task<Result<TUser, Error>>> fetchUser)
        {
            PurgeExpiredLookupCacheEntries();

            if (TryGetLookupCache(cacheKey, out var cachedUser, out var cachedError))
            {
                return cachedUser != null
                    ? Result.Success<TUser, Error>(cachedUser)
                    : Result.Failure<TUser, Error>(cachedError!);
            }

            var fetchResult = await fetchUser();
            if (fetchResult.IsSuccess && fetchResult.Value != null)
            {
                CacheLookupCache(cacheKey, fetchResult.Value);
                CacheProviderUser(fetchResult.Value);
                return fetchResult;
            }

            if (fetchResult.IsFailure && IsCacheableDirectoryLookupFailure(fetchResult.Error))
            {
                CacheLookupCache(cacheKey, fetchResult.Error!);
            }

            return fetchResult;
        }

        private void CacheLookupCache(string cacheKey, TUser user)
        {
            _lookupCache[cacheKey] = new CachedDirectoryUser(user, DateTimeOffset.UtcNow);
        }

        private void CacheLookupCache(string cacheKey, Error error)
        {
            _lookupCache[cacheKey] = new CachedDirectoryUser(error, DateTimeOffset.UtcNow);
        }

        private bool TryGetLookupCache(string cacheKey, out TUser? user, out Error? error)
        {
            _lookupCache.TryGetValue(cacheKey, out var cachedEntry);

            if (cachedEntry != null && !IsLookupCacheExpired(cachedEntry))
            {
                user = cachedEntry.IsFound ? cachedEntry.User : default;
                error = cachedEntry.ErrorCode == null ? null : new Error(cachedEntry.ErrorCode,
                    cachedEntry.ErrorMessage ?? "User not found in directory cache.");
                return true;
            }

            if (cachedEntry != null)
            {
                _lookupCache.TryRemove(cacheKey, out _);
            }

            user = default;
            error = null;
            return false;
        }

        private static bool IsLookupKeyInvalid(string lookupKey)
        {
            return string.IsNullOrWhiteSpace(lookupKey);
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

        private bool IsLookupCacheExpired(CachedDirectoryUser cachedDirectoryUser)
        {
            return DateTimeOffset.UtcNow - cachedDirectoryUser.CachedAtUtc > _fallbackCacheTtl;
        }

        private void PurgeExpiredLookupCacheEntries()
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

                foreach (var key in _lookupCache.Keys)
                {
                    if (_lookupCache.TryGetValue(key, out var entry) && IsLookupCacheExpired(entry))
                    {
                        _lookupCache.TryRemove(key, out _);
                    }
                }

                _nextFallbackCacheSweepUtc = now + _fallbackCacheSweepInterval;
            }
        }

        private static string BuildLookupKey(string keyType, string? key)
        {
            return $"{keyType}/{key}";
        }

        private static string BuildCredentialsLookupKey(string? username, string? domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return username ?? string.Empty;
            }

            return $"{domain}:{username}";
        }

        private TUser? TryGetProviderCachedUser(Func<TUser, bool> predicate)
        {
            return _users.Values.SelectMany(providerUsers => providerUsers.Values).FirstOrDefault(predicate);
        }

        private void CacheProviderUser(TUser user)
        {
            var providerKey = user.ProviderName ?? string.Empty;
            var subjectIdKey = user.ProviderSubjectId ?? string.Empty;

            _users.AddOrUpdate(providerKey,
                new ConcurrentDictionary<string, TUser> { [subjectIdKey] = user },
                (_, existingUsers) =>
                {
                    existingUsers[subjectIdKey] = user;
                    return existingUsers;
                });
        }

        private List<Claim> FilterClaims(List<Claim> claims)
        {
            var mapped = claims.Select(claim =>
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

            return ClaimNormalization.CollapseSingleValuedDisplayClaims(mapped);
        }
    }
}
