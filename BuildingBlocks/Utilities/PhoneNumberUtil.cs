using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Utilities
{
    using System;
    using System.Text.RegularExpressions;

    public static class PhoneNumberNormalizer
    {
        /// <summary>
        /// Normalizes a phone number by removing non-digit characters and ensuring it starts with the specified country code.
        /// Handles cases where the phone number starts with a '+' or '00' for international formats.
        /// </summary>
        /// <param name="phoneNumber">The phone number to normalize.</param>
        /// <param name="countryCode">The country code to prepend if not present (e.g., "1" for USA, "44" for UK).</param>
        /// <returns>The normalized phone number.</returns>
        /// <exception cref="ArgumentException">Thrown when phoneNumber or countryCode is null or empty.</exception>
        public static string NormalizePhoneNumber(string phoneNumber, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));

            // Remove all non-digit characters except the leading '+'
            string cleanedNumber = Regex.Replace(phoneNumber.Trim(), @"(?!^\+)\D", "");

            // If the phone number starts with '+', remove it and use the number as is
            if (cleanedNumber.StartsWith("+"))
            {
                cleanedNumber = cleanedNumber.Substring(1);
                return cleanedNumber;
            }

            // If the phone number starts with '00', replace it with '+' and remove '00'
            if (cleanedNumber.StartsWith("00"))
            {
                cleanedNumber = cleanedNumber.Substring(2);
                return cleanedNumber;
            }

            // Remove leading zeros
            cleanedNumber = cleanedNumber.TrimStart('0');

            // Check if the phone number already starts with the country code
            if (cleanedNumber.StartsWith(countryCode))
            {
                return cleanedNumber;
            }

            // Prepend the country code
            return $"{countryCode}{cleanedNumber}";
        }
    }

}
