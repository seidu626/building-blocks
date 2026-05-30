using System.Linq;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Common;

public enum IdentifierType
{
    Email,
    Phone,
    Username
}

public static class IdentifierTypeChecker
{
    public static string GetLdapAttribute(this IdentifierType identifierType)
    {
        switch (identifierType)
        {
            case IdentifierType.Username: return "sAMAccountName";
            case IdentifierType.Email: return "mail";
            case IdentifierType.Phone: return "mobile";
            default: return "sAMAccountName";
        }
    }

    public static IdentifierType GetIdentifierType(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input), "Input cannot be null or empty.");
        }

        // Trim the input to remove any leading/trailing spaces
        input = input.Trim();

        // Regular expression for email validation
        var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

        // Remove common dialling extensions and punctuation, keep only digits for phone detection.
        var normalized = GetNormalizedDigits(input);
        var isPhoneLike = IsLikelyPhoneNumber(input);

        // Check if the input is an email
        if (emailRegex.IsMatch(input))
        {
            return IdentifierType.Email;
        }

        // Check if the input is a phone number
        if (isPhoneLike && normalized.Length >= 7 && normalized.Length <= 20)
        {
            return IdentifierType.Phone;
        }

        // If it's neither an email nor a phone number, consider it as a username
        return IdentifierType.Username;
    }

    private static bool IsLikelyPhoneNumber(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var sanitized = Regex.Replace(input.Trim(), ExtensionPattern, string.Empty, RegexOptions.IgnoreCase);

        return PhoneCharacterRegex.IsMatch(sanitized);
    }

    private static string GetNormalizedDigits(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sanitized = Regex.Replace(input.Trim(), ExtensionPattern, string.Empty, RegexOptions.IgnoreCase);
        var digitsOnly = new string(sanitized.Where(char.IsDigit).ToArray());

        return digitsOnly;
    }

    private const string ExtensionPattern = "(?:\\s*(?:ext|x|extension)\\s*\\d+\\s*)$";
    private const string PhoneCharacterPattern = @"^[+\s()./\d-]+$";
    private static readonly Regex PhoneCharacterRegex = new(PhoneCharacterPattern, RegexOptions.Compiled);
}
