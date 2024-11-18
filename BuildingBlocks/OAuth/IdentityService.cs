using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.Caching;
using BuildingBlocks.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.OAuth;
#nullable enable
public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IDiscoveryCache _discoveryCache;
    private readonly ILogger<IdentityService> _logger;
    private readonly ICacheProvider _cacheProvider;

    public IdentityService(
        IHttpContextAccessor contextAccessor,
        IDiscoveryCache discoveryCache,
        ILogger<IdentityService> logger,
        ICacheProvider cacheProvider)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        _discoveryCache = discoveryCache ?? throw new ArgumentNullException(nameof(discoveryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
    }

    public IDictionary<string, string> ExtractClaims(IList<string> fieldNames, IList<Claim>? claims = null)
    {
        claims ??= _contextAccessor.HttpContext?.User.Claims.ToList();
        return claims == null
            ? new Dictionary<string, string>()
            : claims.Where(claim => fieldNames.Contains(claim.Type))
                .ToDictionary(claim => claim.Type, claim => claim.Value);
    }

    public IDictionary<string, string> ExtractClaims(IList<string> fieldNames, ClaimsPrincipal? userClaims = null)
    {
        var claims = userClaims?.Claims ?? _contextAccessor.HttpContext?.User.Claims;
        if (claims == null) return new Dictionary<string, string>();

        return claims
            .Where(claim => fieldNames.Contains(claim.Type))
            .ToDictionary(claim => claim.Type, claim => claim.Value);
    }

    public IList<Claim> GetUserClaims() =>
        _contextAccessor.HttpContext?.User.Claims.ToList() ?? new List<Claim>();

    public string? GetSpecificClaim(string claimType, IList<Claim>? claims = null) =>
        claims?.FirstOrDefault(claim => claim.Type == claimType)?.Value ??
        _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;

    public async ValueTask<string?> GetUserIdentityAsync(CancellationToken cancellationToken)
    {
        var user = await GetUserInfoAsync(cancellationToken);
        return user?.Sub;
    }

    public async ValueTask<string?>
        GetUsernameAsync(CancellationToken cancellationToken, IList<Claim>? claims = null) =>
        GetSpecificClaim("preferred_username", claims) ??
        (await GetUserInfoAsync(cancellationToken))?.PreferredUsername;

    public async ValueTask<IdentityUser?> GetUserInfoAsync(CancellationToken cancellationToken,
        HttpContext? httpContext = null)
    {
        try
        {
            var token = await GetAccessTokenAsync(cancellationToken, httpContext);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Token is null");
                return null;
            }

            var cachedUser = await _cacheProvider.GetAsync<IdentityUser>(token, cancellationToken);
            if (cachedUser != null) return cachedUser;

            var disco = await _discoveryCache.GetAsync();
            using var client = new HttpClient();
            var userInfoResponse = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = token
            }, cancellationToken);

            if (userInfoResponse.IsError || userInfoResponse.Raw == null)
            {
                _logger.LogError(userInfoResponse.Exception?.Demystify(),
                    "Error fetching user info from {Endpoint}", disco.UserInfoEndpoint);
                return null;
            }

            var user = userInfoResponse.Raw.TryDeserialize<IdentityUser>(_logger);
            if (user == null) return null;

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            await _cacheProvider.SetAsync(token, user, cacheOptions, cancellationToken);

            return user;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON error during user info retrieval: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user info retrieval: {Message}", ex.Message);
        }

        return null;
    }

    public async Task<string?> GetUserEmailAsync(CancellationToken cancellationToken, IList<Claim>? claims = null) =>
        GetSpecificClaim("email", claims) ?? (await GetUserInfoAsync(cancellationToken))?.Email;

    public async ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken,
        HttpContext? httpContext = null)
    {
        var context = httpContext ?? _contextAccessor.HttpContext;
        if (context != null) return await context.GetTokenAsync("access_token");
        _logger.LogError("HttpContext is null. This method must be called within the context of an HTTP request.");
        return null;
    }
}