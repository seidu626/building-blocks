namespace BuildingBlocks.Ldap;

using System.Text;

internal static class PhoneLookupHelper
{
    public static IReadOnlyList<string> GetPhoneLookupCandidates(string? phone)
    {
        var candidates = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(phone))
        {
            return Array.Empty<string>();
        }

        var trimmedPhone = phone.Trim();
        if (!string.IsNullOrWhiteSpace(trimmedPhone))
        {
            candidates.Add(trimmedPhone);
        }

        var normalized = NormalizePhoneToDigits(phone);
        if (!string.IsNullOrEmpty(normalized))
        {
            candidates.Add(normalized);

            if (normalized.StartsWith('0') && normalized.Length > 1)
            {
                candidates.Add(normalized[1..]);
            }
            else
            {
                candidates.Add("0" + normalized);
            }

            candidates.Add($"+{normalized}");
            if (normalized.Length > 2 && normalized.StartsWith("00"))
            {
                candidates.Add(normalized[2..]);
            }
        }

        return candidates.ToArray();
    }

    public static string? NormalizePhoneToDigits(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        var builder = new StringBuilder();
        foreach (var character in phone)
        {
            if (char.IsDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    public static bool IsNormalizedPhoneMatch(string? phone, string? normalizedPhone)
    {
        if (string.IsNullOrEmpty(normalizedPhone))
        {
            return false;
        }

        var normalizedCandidate = NormalizePhoneToDigits(phone);
        return !string.IsNullOrEmpty(normalizedCandidate) && string.Equals(normalizedCandidate, normalizedPhone, StringComparison.Ordinal);
    }
}
