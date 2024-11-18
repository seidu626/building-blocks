using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Exceptions;
using CSharpFunctionalExtensions;
using IdentityModel;
using System.Collections.Concurrent;

namespace BuildingBlocks.Ldap.UserStore
{
    public class InMemoryUserStore<TUser> : ILdapUserStore<TUser> where TUser : IAppUser, new()
    {
        private readonly ILdapService<TUser> _authenticationService;

        // In-memory data store using thread-safe concurrent dictionary
        private readonly ConcurrentDictionary<string?, ConcurrentDictionary<string?, TUser>> _users =
            new ConcurrentDictionary<string?, ConcurrentDictionary<string?, TUser>>();

        public InMemoryUserStore(ILdapService<TUser> authenticationService)
        {
            _authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string? username, string password)
        {
            return await ValidateCredentialsAsync(username, password, null);
        }

        public async Task<Result<TUser, Error>> ValidateCredentialsAsync(string? username, string password,
            string? domain)
        {
            return await PerformAuthenticationAsync(() => _authenticationService.Login(username, password, domain));
        }

        public async Task<Result<TUser, Error>> FindBySubjectIdAsync(string? subjectId)
        {
            return await FindAndCacheAsync(u => u.SubjectId == subjectId,
                () => _authenticationService.FindUser(subjectId.Replace("ldap_", "")));
        }

        public async Task<Result<TUser, Error>> FindByUsernameAsync(string? username)
        {
            return await FindAndCacheAsync(u => u.Username == username,
                () => _authenticationService.FindUser(username.Replace("ldap_", "")));
        }

        public async Task<Result<TUser, Error>> FindByEmailAsync(string? email)
        {
            return await FindAndCacheAsync(u => u.Email == email,
                () => _authenticationService.FindUserByEmail(email.Replace("ldap_", "")));
        }

        public async Task<Result<TUser, Error>> FindByPhoneAsync(string? phone)
        {
            return await FindAndCacheAsync(u => u.Mobile == phone,
                () => _authenticationService.FindUserByPhone(phone.Replace("ldap_", "")));
        }

        public async Task<Result<List<string>, Error>> GetUserAttributesAsync(string attributeName,
            string? attributeValue, string domain)
        {
            return await _authenticationService.GetUserAttributes(attributeName, attributeValue, domain);
        }

        public async Task<Result<TUser, Error>> FindByExternalProviderAsync(string? provider, string userId)
        {
            if (_users.TryGetValue(provider, out var providerUsers) && providerUsers.TryGetValue(userId, out var user))
            {
                return Result.Success<TUser, Error>(user);
            }

            return Result.Failure<TUser, Error>(new Error("404", "User not found in external provider"));
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

            _users.AddOrUpdate(provider,
                new ConcurrentDictionary<string?, TUser> { [userId] = user },
                (key, existingUsers) =>
                {
                    existingUsers[userId] = user;
                    return existingUsers;
                });

            return user;
        }

        private async Task<Result<TUser, Error>> PerformAuthenticationAsync(
            Func<Task<Result<TUser, Error>>> authenticationFunc)
        {
            try
            {
                return await authenticationFunc();
            }
            catch (LoginFailedException ex)
            {
                return Result.Failure<TUser, Error>(new Error("404", ex.Message, ex));
            }
        }

        private async Task<Result<TUser, Error>> FindAndCacheAsync(Func<TUser, bool> predicate,
            Func<Task<Result<TUser, Error>>> fetchUser)
        {
            var cachedUser = _users.Values.SelectMany(providerUsers => providerUsers.Values).FirstOrDefault(predicate);

            if (cachedUser != null)
            {
                return cachedUser;
            }

            var fetchResult = await fetchUser();
            if (fetchResult.IsFailure)
            {
                return Result.Failure<TUser, Error>(fetchResult.Error);
            }

            var fetchedUser = fetchResult.Value;

            _users.AddOrUpdate(fetchedUser.ProviderName,
                new ConcurrentDictionary<string?, TUser> { [fetchedUser.ProviderSubjectId] = fetchedUser },
                (key, existingUsers) =>
                {
                    existingUsers[fetchedUser.ProviderSubjectId] = fetchedUser;
                    return existingUsers;
                });

            return fetchedUser;
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