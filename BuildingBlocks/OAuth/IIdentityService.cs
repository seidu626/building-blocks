#nullable disable
using System.Security.Claims;

namespace BuildingBlocks.OAuth;

public interface IIdentityService
{
    IDictionary<string, string> ExtractClaims(IList<string> fieldNames, IList<Claim> claims = null);
    IList<Claim> GetUserClaims();
    string GetSpecificClaim(string claimType, IList<Claim> claims = null);
    ValueTask<string> GetUserIdentityAsync(CancellationToken cancellationToken = default);
    ValueTask<string> GetUsernameAsync(CancellationToken cancellationToken = default, IList<Claim> claims = null);
    Task<string> GetUserEmailAsync(CancellationToken cancellationToken = default, IList<Claim> claims = null);
    ValueTask<IdentityUser> GetUserInfoAsync(CancellationToken cancellationToken = default);
    ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}