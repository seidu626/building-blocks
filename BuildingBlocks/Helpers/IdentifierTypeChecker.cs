namespace BuildingBlocks.Helpers;

using System;
using System.Text.RegularExpressions;

public enum IdentifierType
{
    Email,
    Phone,
    Username
}

public class IdentifierTypeChecker
{
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