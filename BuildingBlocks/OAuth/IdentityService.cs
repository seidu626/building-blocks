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
        _discoveryCache = discoveryCache;
        _logger = logger;
        _cacheProvider = cacheProvider;
    }

    public IDictionary<string, string> ExtractClaims(IList<string> fieldNames, IList<Claim> claims = null)
    {
        if (claims == null)
        {
            claims = _contextAccessor.HttpContext?.User.Claims.ToList();
        }

        if (claims == null)
        {
            return new Dictionary<string, string>();
        }

        return claims.Where(claim => fieldNames.Contains(claim.Type))
            .ToDictionary(claim => claim.Type, claim => claim.Value);
    }

    public IList<Claim> GetUserClaims()
    {
        return _contextAccessor.HttpContext?.User.Claims.ToList() ?? new List<Claim>();
    }

    public string GetSpecificClaim(string claimType, IList<Claim> claims = null)
    {
        return claims?.FirstOrDefault(claim => claim.Type == claimType)?.Value
               ?? _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
    }

    public async ValueTask<string> GetUserIdentityAsync(CancellationToken cancellationToken)
    {
        var user = await GetUserInfoAsync(cancellationToken);
        return user?.Sub;
    }

    public async ValueTask<string> GetUsernameAsync(CancellationToken cancellationToken, IList<Claim> claims = null)
    {
        return GetSpecificClaim("preferred_username", claims)
               ?? (await GetUserInfoAsync(cancellationToken))?.PreferredUsername;
    }


    public async ValueTask<IdentityUser> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        string userinfoResponse = "";
        try
        {
            string token = await this.GetAccessTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                this._logger.LogInformation("Token is null");
                return (IdentityUser)null;
            }

            IdentityUser record = await this._cacheProvider.GetAsync<IdentityUser>(token, cancellationToken);
            if (record != (IdentityUser)null)
                return record;
            DiscoveryDocumentResponse disco = await this._discoveryCache.GetAsync();
            using (HttpClient client1 = new HttpClient())
            {
                HttpClient client2 = client1;
                UserInfoRequest request = new UserInfoRequest();
                request.Address = disco.UserInfoEndpoint;
                request.Token = token;
                CancellationToken cancellationToken1 = cancellationToken;
                UserInfoResponse userInfoAsync = await client2.GetUserInfoAsync(request, cancellationToken1);
                userinfoResponse = userInfoAsync.Raw;
                this._logger.LogInformation("USERINFO_RESPONSE: " + userinfoResponse);
                if (userInfoAsync.IsError || userinfoResponse == null)
                {
                    this._logger.LogError(userInfoAsync.Exception.Demystify<Exception>(),
                        "Error fetching user info from {Endpoint}", (object)disco.UserInfoEndpoint);
                    return (IdentityUser)null;
                }

                record = userinfoResponse.TryDeserialize<IdentityUser>((ILogger)this._logger);
                if (record == (IdentityUser)null)
                    return (IdentityUser)null;
                ICacheProvider cacheProvider = this._cacheProvider;
                string key = token;
                IdentityUser identityUser = record;
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                options.AbsoluteExpirationRelativeToNow = new TimeSpan?(TimeSpan.FromMinutes(60.0));
                CancellationToken cancellationToken2 = cancellationToken;
                await cacheProvider.SetAsync<IdentityUser>(key, identityUser, options, cancellationToken2);
                return record;
            }
        }
        catch (JsonException ex)
        {
            if (((Exception)ex).InnerException != null)
                this._logger.LogError((Exception)ex, ((Exception)ex).InnerException.Message);
            this._logger.LogInformation("USERINFO_RESPONSE: " + userinfoResponse);
            this._logger.LogError((Exception)ex, ((Exception)ex).Message);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
                this._logger.LogError(ex, ex.InnerException.Message);
            this._logger.LogInformation("USERINFO_RESPONSE: " + userinfoResponse);
            this._logger.LogError(ex, ex.Message);
        }

        return (IdentityUser)null;
    }


    public async Task<string> GetUserEmailAsync(CancellationToken cancellationToken, IList<Claim> claims = null)
    {
        return GetSpecificClaim("email", claims)
               ?? (await GetUserInfoAsync(cancellationToken))?.Email;
    }

    public async ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogError(
                "HttpContext is null. This method must be called within the context of an HTTP request.");
            return null;
        }

        return await httpContext.GetTokenAsync("access_token");
    }
}