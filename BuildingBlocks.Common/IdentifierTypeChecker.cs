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

        // Regular expression for phone number validation (supports international numbers)
        var phoneRegex = new Regex(@"^\+?[0-9]{7,15}$");

        // Check if the input is an email
        if (emailRegex.IsMatch(input))
        {
            return IdentifierType.Email;
        }

        // Check if the input is a phone number
        if (phoneRegex.IsMatch(input))
        {
            return IdentifierType.Phone;
        }

        // If it's neither an email nor a phone number, consider it as a username
        return IdentifierType.Username;
    }
}