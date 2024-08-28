using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Exceptions;
using CSharpFunctionalExtensions;
using IdentityModel;

namespace BuildingBlocks.Ldap.UserStore
{
    public class InMemoryUserStore<TUser> : ILdapUserStore<TUser> where TUser : IAppUser, new()
    {
        private readonly ILdapService<TUser> _authenticationService;

        private readonly Dictionary<string, Dictionary<string, TUser>> _users =
            new Dictionary<string, Dictionary<string, TUser>>();

        public InMemoryUserStore(ILdapService<TUser> authenticationService)
        {
            _authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public Result<TUser, Error> ValidateCredentials(string username, string password)
        {
            return ValidateCredentials(username, password, null);
        }

        public Result<TUser, Error> ValidateCredentials(string username, string password, string domain)
        {
            return PerformAuthentication(() => _authenticationService.Login(username, password, domain));
        }

        public Result<TUser, Error> FindBySubjectId(string subjectId)
        {
            return FindAndCache(u => u.SubjectId == subjectId,
                () => _authenticationService.FindUser(subjectId.Replace("ldap_", "")));
        }

        public Result<TUser, Error> FindByUsername(string username)
        {
            return FindAndCache(u => u.Username == username,
                () => _authenticationService.FindUser(username.Replace("ldap_", "")));
        }

        public Result<TUser, Error> FindByEmail(string email)
        {
            return FindAndCache(u => u.Email == email,
                () => _authenticationService.FindUserByEmail(email.Replace("ldap_", "")));
        }

        public Result<List<string>, Error> GetUserAttributes(string attributeName, string attributeValue, string domain)
        {
            return _authenticationService.GetUserAttributes(attributeName, attributeValue, domain);
        }

        public Result<TUser, Error> FindByExternalProvider(string provider, string userId)
        {
            if (_users.TryGetValue(provider, out var providerUsers) && providerUsers.TryGetValue(userId, out var user))
            {
                return Result.Success<TUser, Error>(user);
            }

            return Result.Failure<TUser, Error>(new Error("404", "User not found", null));
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

            if (!_users.ContainsKey(provider))
            {
                _users[provider] = new Dictionary<string, TUser>();
            }

            _users[provider][userId] = user;

            return Result.Success<TUser, Error>(user);
        }

        private Result<TUser, Error> PerformAuthentication(Func<Result<TUser, Error>> authenticationFunc)
        {
            try
            {
                return authenticationFunc();
            }
            catch (LoginFailedException ex)
            {
                return Result.Failure<TUser, Error>(new Error("404", ex.Message, ex));
            }
        }

        private Result<TUser, Error> FindAndCache(Func<TUser, bool> predicate, Func<Result<TUser, Error>> fetchUser)
        {
            var cachedUser = _users.Values.SelectMany(providerUsers => providerUsers.Values).FirstOrDefault(predicate);

            if (cachedUser != null)
            {
                return Result.Success<TUser, Error>(cachedUser);
            }

            var fetchResult = fetchUser();
            if (fetchResult.IsFailure)
            {
                return Result.Failure<TUser, Error>(fetchResult.Error);
            }

            var fetchedUser = fetchResult.Value;

            if (!_users.ContainsKey(fetchedUser.SubjectId))
            {
                _users[fetchedUser.SubjectId] = new Dictionary<string, TUser>();
            }

            _users[fetchedUser.SubjectId][fetchedUser.Username] = fetchedUser;

            return Result.Success<TUser, Error>(fetchedUser);
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