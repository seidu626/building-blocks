#nullable disable
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.OAuth;
#nullable enable
public interface IIdentityService
{
    IDictionary<string, string> ExtractClaims(IList<string> fieldNames, IList<Claim>? claims = null);
    IDictionary<string, string> ExtractClaims(IList<string> fieldNames, ClaimsPrincipal? userClaims = null);
    IList<Claim> GetUserClaims();
    string? GetSpecificClaim(string claimType, IList<Claim>? claims = null);
    ValueTask<string?> GetUserIdentityAsync(CancellationToken cancellationToken = default);
    ValueTask<string?> GetUsernameAsync(CancellationToken cancellationToken = default, IList<Claim>? claims = null);
    Task<string?> GetUserEmailAsync(CancellationToken cancellationToken = default, IList<Claim>? claims = null);
    ValueTask<IdentityUser?> GetUserInfoAsync(CancellationToken cancellationToken, HttpContext? httpContext = null);
    ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default, HttpContext? httpContext = default);
}