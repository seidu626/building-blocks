namespace BuildingBlocks.Ldap;

using System.Text.RegularExpressions;
using System.Text;

internal static class PhoneLookupHelper
{
    private const int MaxSignificantDigits = 15;
    private const int MinSignificantDigits = 7;
    private const string ExtensionPattern = "(?:\\s*(?:ext|x|extension)\\s*\\d+\\s*)$";

    private static readonly Regex ExtensionRegex = new(ExtensionPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static IReadOnlyList<string> GetPhoneLookupCandidates(string? phone)
    {
        var candidates = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(phone))
        {
            return Array.Empty<string>();
        }

        var trimmedPhone = TrimPhoneForLookup(phone);
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

        phone = TrimPhoneForLookup(phone);
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
        if (string.IsNullOrWhiteSpace(normalizedCandidate))
        {
            return false;
        }

        if (string.Equals(normalizedCandidate, normalizedPhone, StringComparison.Ordinal))
        {
            return true;
        }

        if (normalizedCandidate.Length < MinSignificantDigits || normalizedPhone.Length < MinSignificantDigits)
        {
            return false;
        }

        var maxSuffixLength = Math.Min(
            Math.Min(normalizedCandidate.Length, normalizedPhone.Length),
            MaxSignificantDigits);

        for (var i = maxSuffixLength; i >= MinSignificantDigits; i--)
        {
            var candidateSuffix = normalizedCandidate.AsSpan(normalizedCandidate.Length - i);
            var inputSuffix = normalizedPhone.AsSpan(normalizedPhone.Length - i);

            if (candidateSuffix.SequenceEqual(inputSuffix))
            {
                return true;
            }
        }

        return false;
    }

    private static string? TrimPhoneForLookup(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return phone;
        }

        return ExtensionRegex.Replace(phone.Trim(), string.Empty);
    }
}
