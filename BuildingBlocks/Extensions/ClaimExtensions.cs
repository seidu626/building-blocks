// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.ClaimExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Security.Claims;

namespace BuildingBlocks.Extensions;

public static class ClaimExtensions
{
  public static IDictionary<string, string> ExtractClaims(
    this IList<Claim> claims,
    IList<string> fieldNames)
  {
    if (claims == null)
      throw new ArgumentNullException(nameof (claims));
    Dictionary<string, string> claims1 = new Dictionary<string, string>();
    foreach (string fieldName1 in (IEnumerable<string>) fieldNames)
    {
      string fieldName = fieldName1;
      Claim claim = claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == fieldName));
      if (claim != null)
        claims1[claim.Type] = claim.Value;
    }
    return (IDictionary<string, string>) claims1;
  }

  public static string ExtractSingleClaim(this IList<Claim> claims, string fieldName)
  {
    if (claims == null)
      throw new ArgumentNullException(nameof (claims));
    return claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == fieldName))?.Value;
  }
}