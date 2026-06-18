using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Ldap.UserStore
{
    /// <summary>
    /// Normalizes claim lists before they are cached / returned for token issuance.
    /// </summary>
    internal static class ClaimNormalization
    {
        private static readonly Regex NestedBracket = new(@"\[[^\]]*\[", RegexOptions.Compiled);

        // Display claim types that are conceptually single-valued. Dirty/duplicated upstream
        // displayName data (e.g. an HR/AD sync that re-wraps the value into "Name [ Org - Dept ]"
        // and appends variants over time) can otherwise surface as several `name` claims, which
        // clients render concatenated.
        private static readonly string[] SingleValuedDisplayTypes = { "name" };

        /// <summary>
        /// Collapses each single-valued display claim type to one value, in place. Prefers values
        /// without a nested-bracket corruption marker; otherwise keeps the most recent (last).
        /// </summary>
        public static List<Claim> CollapseSingleValuedDisplayClaims(List<Claim> claims)
        {
            foreach (var type in SingleValuedDisplayTypes)
            {
                var matching = claims.Where(c => c.Type == type).ToList();
                if (matching.Count <= 1)
                {
                    continue;
                }

                var chosen = PickDisplayValue(matching.Select(c => c.Value));
                claims.RemoveAll(c => c.Type == type);
                if (!string.IsNullOrWhiteSpace(chosen))
                {
                    claims.Add(new Claim(type, chosen));
                }
            }

            return claims;
        }

        private static string PickDisplayValue(IEnumerable<string> values)
        {
            var distinct = values
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.Trim())
                .Distinct()
                .ToList();

            if (distinct.Count == 0)
            {
                return string.Empty;
            }

            var clean = distinct.Where(v => !NestedBracket.IsMatch(v)).ToList();
            var pool = clean.Count > 0 ? clean : distinct;
            return pool[pool.Count - 1];
        }
    }
}
