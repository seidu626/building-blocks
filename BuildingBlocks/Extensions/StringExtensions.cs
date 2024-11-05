using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Utilities.ObjectPools;

namespace BuildingBlocks.Extensions;

/// <summary>
/// Extensions for <see cref="string"/>
/// </summary>
public static class StringExtensions
{
    private static readonly char[] InvalidFileNameCharacters = Path.GetInvalidFileNameChars();
    private static readonly char[] InvalidPathCharacters = Path.GetInvalidPathChars();
    public const string CarriageReturnLineFeed = "\r\n";
    public const string Empty = "";
    public const char CarriageReturn = '\r';
    public const char LineFeed = '\n';
    public const char Tab = '\t';


    #region Char extensions

    [DebuggerStepThrough]
    public static int ToInt(this char value)
    {
        if (value >= '0' && value <= '9')
        {
            return value - '0';
        }
        else if (value >= 'a' && value <= 'f')
        {
            return (value - 'a') + 10;
        }
        else if (value >= 'A' && value <= 'F')
        {
            return (value - 'A') + 10;
        }

        return -1;
    }

    [DebuggerStepThrough]
    public static string ToUnicode(this char c)
    {
        using (var w = new StringWriter(CultureInfo.InvariantCulture))
        {
            WriteCharAsUnicode(c, w);
            return w.ToString();
        }
    }

    internal static void WriteCharAsUnicode(char c, TextWriter writer)
    {
        Guard.AgainstNull(writer, "writer");

        char h1 = ((c >> 12) & '\x000f').ToHex();
        char h2 = ((c >> 8) & '\x000f').ToHex();
        char h3 = ((c >> 4) & '\x000f').ToHex();
        char h4 = (c & '\x000f').ToHex();

        writer.Write('\\');
        writer.Write('u');
        writer.Write(h1);
        writer.Write(h2);
        writer.Write(h3);
        writer.Write(h4);
    }

    public static char TryRemoveDiacritic(this char c)
    {
        var normalized = c.ToString().Normalize(NormalizationForm.FormD);
        if (normalized.Length > 1)
        {
            return normalized[0];
        }

        return c;
    }

    #endregion
        
    /// <summary>
    /// Creates a SHA256 hash of the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>A hash</returns>
    public static string ToSha256(this string input)
    {
        if (input.IsNullOrEmptyOrWhiteSpace()) return string.Empty;

        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// Creates a SHA512 hash of the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>A hash</returns>
    public static string ToSha512(this string input)
    {
        if (input.IsNullOrEmptyOrWhiteSpace()) return string.Empty;

        using (var sha = SHA512.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
    /// <summary>
    /// A nicer way of calling <see cref="string.IsNullOrEmpty(string)"/>
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

    /// <summary>
    /// A nice way of calling the inverse of <see cref="string.IsNullOrEmpty(string)"/>
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNotNullOrEmpty(this string value) => !value.IsNullOrEmpty();

    /// <summary>
    /// A nice way of checking if a string is null, empty or whitespace 
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNullOrEmptyOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// A nice way of checking the inverse of (if a string is null, empty or whitespace) 
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNotNullOrEmptyOrWhiteSpace(this string value)
        => !value.IsNullOrEmptyOrWhiteSpace();

    /// <summary>
    /// Parses a string as Boolean, valid inputs are: <c>true|false|yes|no|1|0</c>.
    /// <remarks>Input is parsed as Case-Insensitive.</remarks>
    /// </summary>
    public static bool TryParseAsBool(this string value, out bool result)
    {
        Ensure.NotNull(value, nameof(value));

        const StringComparison compPolicy = StringComparison.OrdinalIgnoreCase;

        if (value.Equals("true", compPolicy)
            || value.Equals("yes", compPolicy)
            || value.Equals("1", compPolicy))
        {
            result = true;
            return true;
        }

        if (value.Equals("false", compPolicy)
            || value.Equals("no", compPolicy)
            || value.Equals("0", compPolicy))
        {
            result = false;
            return true;
        }

        result = false;
        return false;
    }

    /// <summary>
    /// Allows for using strings in <see langword="null"/> coalescing operations.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <returns>
    /// Null if <paramref name="value"/> is empty or the original <paramref name="value"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static string NullIfEmpty(this string value) => value == string.Empty ? null : value;
        
    /// <summary>
    /// Determines whether the string is null, empty or all whitespace.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Determines whether the string is all white space. Empty string will return false.
    /// </summary>
    /// <param name="value">The string to test whether it is all white space.</param>
    /// <returns>
    /// 	<c>true</c> if the string is all white space; otherwise, <c>false</c>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsWhiteSpace(this string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value.Length == 0)
            return false;

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i]))
                return false;
        }

        return true;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValue(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <remarks>to get equivalent result to PHPs md5 function call Hash("my value", Encoding.ASCII, false).</remarks>
    [DebuggerStepThrough]
    public static string Hash(this string value, Encoding encoding, bool toBase64 = false)
    {
        if (value.IsEmpty())
            return value;

        using (var md5 = MD5.Create())
        {
            byte[] data = encoding.GetBytes(value);

            if (toBase64)
            {
                byte[] hash = md5.ComputeHash(data);
                return Convert.ToBase64String(hash);
            }
            else
            {
                return md5.ComputeHash(data).ToHexString().ToLower();
            }
        }
    }

    /// <summary>
    /// Mask by replacing characters with asterisks.
    /// </summary>
    /// <param name="value">The string</param>
    /// <param name="length">Number of characters to leave untouched.</param>
    /// <returns>The mask string</returns>
    [DebuggerStepThrough]
    public static string Mask(this string value, int length)
    {
        if (value.HasValue())
            return value.Substring(0, length) + new String('*', value.Length - length);

        return value;
    }
        
    /// <summary>
    /// Tries to extract the value between the tag <paramref name="tagName"/> 
    /// from the <paramref name="input"/>.
    /// <remarks>This method is case insensitive.</remarks>
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="tagName">The tag whose value will be returned e.g <c>span, img</c>.</param>
    /// <param name="value">The extracted value.</param>
    /// <returns><c>True</c> if successful otherwise <c>False</c>.</returns>
    public static bool TryExtractValueFromTag(this string input, string tagName, out string value)
    {
        Ensure.NotNull(input, nameof(input));
        Ensure.NotNull(tagName, nameof(tagName));

        var pattern = $"<{tagName}[^>]*>(.*)</{tagName}>";
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            value = match.Groups[1].ToString();
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Returns a string array containing the trimmed substrings in this <paramref name="value"/>
    /// that are delimited by the provided <paramref name="separators"/>.
    /// </summary>
    public static string[] SplitAndTrim(this string value, params char[] separators)
    {
        Ensure.NotNull(value, nameof(value));
        return value.Trim()
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToArray();
    }
        
    /// <summary>
    /// Splits a string into a string array
    /// </summary>
    /// <param name="value">String value to split</param>
    /// <param name="separator">If <c>null</c> then value is searched for a common delimiter like pipe, semicolon or comma</param>
    /// <returns>String array</returns>
    [DebuggerStepThrough]
    public static string[] SplitSafe(this string value, string separator)
    {
        if (string.IsNullOrEmpty(value))
            return new string[0];

        // Do not use separator.IsEmpty() here because whitespace like " " is a valid separator.
        // an empty separator "" returns array with value.
        if (separator == null)
        {
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == ';' || c == ',' || c == '|')
                {
                    return value.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (c == '\r' && (i + 1) < value.Length & value[i + 1] == '\n')
                {
                    return value.GetLines(false, true).ToArray();
                }
            }

            return new string[] { value };
        }
        else
        {
            return value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    /// <summary>Splits a string into two strings</summary>
    /// <returns>true: success, false: failure</returns>
    [DebuggerStepThrough]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    public static bool SplitToPair(this string value, out string leftPart, out string rightPart, string delimiter, bool splitAfterLast = false)
    {
        leftPart = value;
        rightPart = "";

        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(delimiter))
        {
            return false;
        }

        var idx = splitAfterLast
            ? value.LastIndexOf(delimiter)
            : value.IndexOf(delimiter);

        if (idx == -1)
        {
            return false;
        }

        leftPart = value.Substring(0, idx);
        rightPart = value.Substring(idx + delimiter.Length);

        return true;
    }

    /// <summary>Debug.WriteLine</summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Dump(this string value, bool appendMarks = false)
    {
        Debug.WriteLine(value);
        Debug.WriteLineIf(appendMarks, "------------------------------------------------");
    }
        
    /// <summary>
    /// Checks if the <paramref name="input"/> contains the <paramref name="stringToCheckFor"/> 
    /// based on the provided <paramref name="comparison"/> rules.
    /// </summary>
    public static bool Contains(this string input, string stringToCheckFor, StringComparison comparison) =>
        input.IndexOf(stringToCheckFor, comparison) >= 0;

    /// <summary>
    /// Checks that given <paramref name="input"/> matches any of the potential matches.
    /// Inspired by: http://stackoverflow.com/a/20644611/23199
    /// </summary>
    public static bool EqualsAny(this string input, StringComparer comparer, string match1, string match2) =>
        comparer.Equals(input, match1) || comparer.Equals(input, match2);

    /// <summary>
    /// Checks that given <paramref name="input"/> matches any of the potential matches.
    /// Inspired by: http://stackoverflow.com/a/20644611/23199
    /// </summary>
    public static bool EqualsAny(this string input, StringComparer comparer, string match1, string match2, string match3) =>
        comparer.Equals(input, match1) || comparer.Equals(input, match2) || comparer.Equals(input, match3);

    /// <summary>
    /// Checks that given <paramref name="input"/> is in a list of 
    /// potential <paramref name="matches"/>.
    /// <remarks>Inspired by: <see href="http://stackoverflow.com/a/20644611/23199"/> </remarks>
    /// </summary>
    public static bool EqualsAny(this string input, StringComparer comparer, params string[] matches) =>
        matches.Any(x => comparer.Equals(x, input));

    /// <summary>
    /// Checks to see if the given input is a valid palindrome or not.
    /// </summary>
    public static bool IsPalindrome(this string input)
    {
        Ensure.NotNull(input, nameof(input));
        var min = 0;
        var max = input.Length - 1;
        while (true)
        {
            if (min > max) { return true; }

            var a = input[min];
            var b = input[max];
            if (char.ToLower(a) != char.ToLower(b)) { return false; }

            min++;
            max--;
        }
    }

    /// <summary>
    /// Truncates the <paramref name="input"/> to the maximum length of <paramref name="maxLength"/> 
    /// and replaces the truncated part with <paramref name="suffix"/>
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="maxLength">Total length of characters to maintain before truncation.</param>
    /// <param name="suffix">The suffix to add to the end of the truncated <paramref name="input"/></param>
    public static string Truncate(this string input, int maxLength, string suffix = "")
    {
        Ensure.NotNull(input, nameof(input));
        Ensure.NotNull(suffix, nameof(suffix));

        if (maxLength < 0) { return input; }
        if (maxLength == 0) { return string.Empty; }

        var chars = input.Take(maxLength).ToArray();

        if (chars.Length != input.Length)
        {
            return new string(chars) + suffix;
        }

        return new string(chars);
    }

    /// <summary>
    /// Removes different types of new lines from a given string.
    /// </summary>
    /// <param name="input">input string.</param>
    /// <returns>The given input minus any new line characters.</returns>
    public static string RemoveNewLines(this string input)
    {
        Ensure.NotNull(input, nameof(input));
        return input.Replace("\n", string.Empty).Replace("\r", string.Empty);
    }

    /// <summary>
    /// Separates a PascalCase string.
    /// </summary>
    /// <example> "ThisIsPascalCase".SeparatePascalCase(); // returns "This Is Pascal Case" </example>
    /// <param name="value">The format to split</param>
    /// <returns>The original string separated on each uppercase character.</returns>
    public static string SeparatePascalCase(this string value)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(value);
        return Regex.Replace(value, "([A-Z])", " $1").Trim();
    }

    /// <summary>
    /// Converts string to Pascal Case
    /// <example>This Is A Pascal Case String.</example>
    /// </summary>
    /// <param name="input">The given input.</param>
    /// <returns>The given <paramref name="input"/> converted to Pascal Case.</returns>
    public static string ToPascalCase(this string input)
    {
        Ensure.NotNull(input, nameof(input));

        var cultureInfo = Thread.CurrentThread.CurrentCulture;
        var textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(input);
    }

    /// <summary>
    /// Compares <paramref name="input"/> against <paramref name="target"/>, 
    /// the comparison is case-sensitive.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="target">The target string</param>
    public static bool IsEqualTo(this string input, string target)
    {
        if (input is null && target is null) { return true; }
        if (input is null || target is null) { return false; }
        if (input.Length != target.Length) { return false; }

        return string.CompareOrdinal(input, target) == 0;
    }
        
    /// <summary>
    /// Formats a string to an invariant culture
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="objects">The objects.</param>
    /// <returns></returns>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormatInvariant(this string format, params object[] objects)
    {
        return string.Format(CultureInfo.InvariantCulture, format, objects);
    }

    /// <summary>
    /// Formats a string to the current culture.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="objects">The objects.</param>
    /// <returns></returns>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormatCurrent(this string format, params object[] objects)
    {
        return string.Format(CultureInfo.CurrentCulture, format, objects);
    }

    /// <summary>
    /// Formats a string to the current UI culture.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="objects">The objects.</param>
    /// <returns></returns>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormatCurrentUI(this string format, params object[] objects)
    {
        return string.Format(CultureInfo.CurrentUICulture, format, objects);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormatWith(this string format, params object[] args)
    {
        return FormatWith(format, CultureInfo.CurrentCulture, args);
    }

    [DebuggerStepThrough]
    public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
    {
        return string.Format(provider, format, args);
    }

    /// <summary>
    /// Handy method to print arguments to <c>System.Console</c>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="args">The arguments.</param>
    [DebuggerStepThrough]
    public static void Print(this string input, params object[] args) => Console.WriteLine(input, args);
        
    /// <summary>
    /// Replaces digits in a string with culture native digits (if digit substitution for culture is required)
    /// </summary>
    [DebuggerStepThrough]
    public static string ReplaceNativeDigits(this string value, IFormatProvider provider = null)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        provider = provider ?? NumberFormatInfo.CurrentInfo;
        var nfi = NumberFormatInfo.GetInstance(provider);

        if (nfi.DigitSubstitution == DigitShapes.None)
        {
            return value;
        }

        var nativeDigits = nfi.NativeDigits;
        var rg = new Regex(@"\d");

        var result = rg.Replace(value, m => nativeDigits[m.Value.ToInt()]);
        return result;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimSafe(this string value)
    {
        return (value.HasValue() ? value.Trim() : value);
    }

    [DebuggerStepThrough]
    public static string Slugify(this string value, bool allowSpace = false, char[] allowChars = null)
    {
        string res = string.Empty;
        var psb = PooledStringBuilder.Rent();

        try
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var sb = (StringBuilder)psb;
                bool space = false;
                char ch;

                for (int i = 0; i < value.Length; ++i)
                {
                    ch = value[i];

                    if (ch == ' ' || ch == '-')
                    {
                        if (allowSpace && ch == ' ')
                            sb.Append(' ');
                        else if (!space)
                            sb.Append('-');
                        space = true;
                        continue;
                    }

                    space = false;

                    if ((ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90) || (ch >= 97 && ch <= 122) || ch == '_')
                    {
                        sb.Append(ch);
                        continue;
                    }

                    if (allowChars != null && allowChars.Contains(ch))
                    {
                        sb.Append(ch);
                        continue;
                    }

                    if ((int)ch >= 128)
                    {
                        switch (ch)
                        {
                            case 'ä': sb.Append("ae"); break;
                            case 'ö': sb.Append("oe"); break;
                            case 'ü': sb.Append("ue"); break;
                            case 'ß': sb.Append("ss"); break;
                            case 'Ä': sb.Append("AE"); break;
                            case 'Ö': sb.Append("OE"); break;
                            case 'Ü': sb.Append("UE"); break;
                            default:
                                var c2 = ch.TryRemoveDiacritic();
                                if ((c2 >= 'a' && c2 <= 'z') || (c2 >= 'A' && c2 <= 'Z'))
                                {
                                    sb.Append(c2);
                                }
                                break;
                        }
                    }
                }   // for

                if (sb.Length > 0)
                {
                    res = sb.ToString().Trim(new char[] { ' ', '-' });

                    Regex pat = new Regex(@"(-{2,})"); // remove double SpaceChar
                    res = pat.Replace(res, "-");
                    res = res.Replace("__", "_");
                }
            }
        }
        catch (Exception ex)
        {
            ex.Dump();
        }
        finally
        {
            psb.Return();
        }

        return (res.Length > 0 ? res : "null");
    }

    /// <summary>
    /// Generates a slug.
    /// <remarks>
    /// Credit goes to <see href="http://stackoverflow.com/questions/2920744/url-slugify-alrogithm-in-cs"/>.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static string GenerateSlug(this string value, uint? maxLength = null)
    {
        // prepare string, remove diacritics, lower case and convert hyphens to whitespace
        var result = RemoveDiacritics(value).Replace("-", " ").ToLowerInvariant();

        result = Regex.Replace(result, @"[^a-z0-9\s-]", string.Empty); // remove invalid characters
        result = Regex.Replace(result, @"\s+", " ").Trim(); // convert multiple spaces into one space

        if (maxLength.HasValue)
        {
            result = result.Substring(0, result.Length <= maxLength
                ? result.Length : (int)maxLength.Value).Trim();
        }
        return Regex.Replace(result, @"\s", "-");
    }
        
    /// <summary>
    /// Splits the input string by carriage return.
    /// </summary>
    /// <param name="input">The string to split</param>
    /// <returns>A sequence with string items per line</returns>
    public static IEnumerable<string> GetLines(this string input, bool trimLines = false, bool removeEmptyLines = false)
    {
        if (input.IsEmpty())
        {
            yield break;
        }

        using (var sr = new StringReader(input))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (trimLines)
                {
                    line = line.Trim();
                }

                if (removeEmptyLines && IsEmpty(line))
                {
                    continue;
                }

                yield return line;
            }
        }
    }

    /// <summary>
    /// Ensure that a string starts with a string.
    /// </summary>
    /// <param name="value">The target string</param>
    /// <param name="startsWith">The string the target string should start with</param>
    /// <returns>The resulting string</returns>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EnsureStartsWith(this string value, string startsWith)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (startsWith == null)
            throw new ArgumentNullException(nameof(startsWith));

        return value.StartsWith(startsWith) ? value : (startsWith + value);
    }

    /// <summary>
    /// Ensures the target string ends with the specified string.
    /// </summary>
    /// <param name="endWith">The target.</param>
    /// <param name="value">The value.</param>
    /// <returns>The target string with the value string at the end.</returns>
    [DebuggerStepThrough]
    public static string EnsureEndsWith(this string value, string endWith)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (endWith == null)
            throw new ArgumentNullException(nameof(endWith));

        if (value.Length >= endWith.Length)
        {
            if (string.Compare(value, value.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                return value;

            string trimmedString = value.TrimEnd(null);

            if (string.Compare(trimmedString, trimmedString.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                return value;
        }

        return value + endWith;
    }

    /// <summary>
    /// Removes the diacritics from the given <paramref name="input"/> 
    /// </summary>
    /// <remarks>
    /// Credit goes to <see href="http://stackoverflow.com/a/249126"/>.
    /// </remarks>
    [DebuggerStepThrough]
    public static string RemoveDiacritics(this string input)
    {
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = StringBuilderCache.Acquire();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return StringBuilderCache.GetStringAndRelease(stringBuilder).Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// A method to convert English digits to Persian numbers.
    /// </summary>
    [DebuggerStepThrough]
    public static string ToPersianNumber(this string input)
    {
        Ensure.NotNull(input, nameof(input));
        return input
            .Replace("0", "۰")
            .Replace("1", "۱")
            .Replace("2", "۲")
            .Replace("3", "۳")
            .Replace("4", "۴")
            .Replace("5", "۵")
            .Replace("6", "۶")
            .Replace("7", "۷")
            .Replace("8", "۸")
            .Replace("9", "۹");
    }

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="xmlInput">The input containing XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>
    /// The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static IEnumerable<XElement> GetElements(this string xmlInput, XName name, bool ignoreCase = true)
    {
        Ensure.NotNull(xmlInput, nameof(xmlInput));
        return xmlInput.GetElements(name, new XmlReaderSettings(), ignoreCase);
    }

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="xmlInput">The input containing XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="settings">The settings used by the <see cref="XmlReader"/></param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>
    /// The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static IEnumerable<XElement> GetElements(this string xmlInput, XName name, XmlReaderSettings settings, bool ignoreCase = true)
    {
        Ensure.NotNull(xmlInput, nameof(xmlInput));
        Ensure.NotNull(name, nameof(name));
        Ensure.NotNull(settings, nameof(settings));

        using var stringReader = new StringReader(xmlInput);
        using var xmlReader = XmlReader.Create(stringReader, settings);

        foreach (var xElement in xmlReader.GetEelements(name, ignoreCase))
        {
            yield return xElement;
        }
    }

    /// <summary>
    /// Compresses the given <paramref name="input"/> to <c>Base64</c> string.
    /// </summary>
    /// <param name="input">The string to be compressed</param>
    /// <returns>The compressed string in <c>Base64</c></returns>
    [DebuggerStepThrough]
    public static string Compress(this string input)
    {
        Ensure.NotNull(input, nameof(input));

        var buffer = Encoding.UTF8.GetBytes(input);
        using var memStream = new MemoryStream();
        using var zipStream = new GZipStream(memStream, CompressionMode.Compress, true);

        zipStream.Write(buffer, 0, buffer.Length);
        zipStream.Close();

        memStream.Position = 0;

        var compressedData = new byte[memStream.Length];
        memStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Decompresses a <c>Base64</c> compressed string.
    /// </summary>
    /// <param name="compressedInput">The string compressed in <c>Base64</c></param>
    /// <returns>The uncompressed string</returns>
    [DebuggerStepThrough]
    public static string Decompress(this string compressedInput)
    {
        Ensure.NotNull(compressedInput, nameof(compressedInput));

        var gZipBuffer = Convert.FromBase64String(compressedInput);
        using var memStream = new MemoryStream();

        var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
        memStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
        memStream.Position = 0;

        var buffer = new byte[dataLength];
        using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
        {
            zipStream.Read(buffer, 0, buffer.Length);
        }

        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// Ensures the given <paramref name="fileName"/> can be used as a file name.
    /// </summary>
    [DebuggerStepThrough]
    public static bool IsValidFileName(this string fileName) =>
        fileName.IsNotNullOrEmptyOrWhiteSpace() && fileName.IndexOfAny(InvalidFileNameCharacters) == -1;

    /// <summary>
    /// Ensures the given <paramref name="path"/> can be used as a path.
    /// </summary>
    [DebuggerStepThrough]
    public static bool IsValidPathName(this string path) =>
        path.IsNotNullOrEmptyOrWhiteSpace() && path.IndexOfAny(InvalidPathCharacters) == -1;

    /// <summary>
    /// Returns a <see cref="Guid"/> from a <c>Base64</c> encoded <paramref name="input"/>.
    /// <example>
    /// DRfscsSQbUu8bXRqAvcWQA== or DRfscsSQbUu8bXRqAvcWQA depending on <paramref name="trimmed"/>.
    /// </example>
    /// <remarks>
    /// See: <see href="https://blog.codinghorror.com/equipping-our-ascii-armor/"/>
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Guid ToGuid(this string input, bool trimmed = true) =>
        trimmed ? new Guid(Convert.FromBase64String(input + "=="))
            : new Guid(Convert.FromBase64String(input));

    /// <summary>
    /// Returns all the start and end indexes of the occurrences of the 
    /// given <paramref name="startTag"/> and <paramref name="endTag"/> 
    /// in the given <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The input to search.</param>
    /// <param name="startTag">The starting tag e.g. <c>&lt;div></c>.</param>
    /// <param name="endTag">The ending tag e.g. <c>&lt;/div></c>.</param>
    /// <returns>
    /// A sequence <see cref="KeyValuePair{TKey,TValue}"/> where the key is 
    /// the starting position and value is the end position.
    /// </returns>
    [DebuggerStepThrough]
    public static IEnumerable<KeyValuePair<int, int>> GetStartAndEndIndexes(this string input, string startTag, string endTag)
    {
        var startIdx = 0;
        int endIdx;

        while ((startIdx = input.IndexOf(startTag, startIdx, StringComparison.Ordinal)) != -1
               && (endIdx = input.IndexOf(endTag, startIdx, StringComparison.Ordinal)) != -1)
        {
            var result = new KeyValuePair<int, int>(startIdx, endIdx);
            startIdx = endIdx;
            yield return result;
        }
    }

    public static string GetFriendlyFilename(this string friendlyName, string guid, string extension)
    {
        var rgx = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9 -]");
        string result = rgx.Replace(friendlyName, "");
        return string.Format("{0}-{1}.{2}", friendlyName, guid, extension);
    }

    public static string Hash(this string input)
    {
        var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
        return string.Concat(hash.Select(b => b.ToString("x2")));
    }

    public static string NullSafeTrim(this string value)
    {
        if (value != null)
        {
            value = value.Trim();
        }

        return value;
    }
        
    /// <summary>
    /// Determines whether this instance and another specified System.String object have the same value.
    /// </summary>
    /// <param name="value">The string to check equality.</param>
    /// <param name="comparing">The comparing with string.</param>
    /// <returns>
    /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
    /// </returns>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCaseSensitiveEqual(this string value, string comparing)
    {
        return string.CompareOrdinal(value, comparing) == 0;
    }

    /// <summary>
    /// Determines whether this instance and another specified System.String object have the same value.
    /// </summary>
    /// <param name="value">The string to check equality.</param>
    /// <param name="comparing">The comparing with string.</param>
    /// <returns>
    /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
    /// </returns>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCaseInsensitiveEqual(this string value, string comparing)
    {
        return string.Compare(value, comparing, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    static char[] _invalids;

    /// <summary>Replaces characters in <c>text</c> that are not allowed in 
    /// file names with the specified replacement character.</summary>
    /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
    /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
    /// <param name="fancy">Whether to replace quotes and slashes with the non-ASCII characters ” and ⁄.</param>
    /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
    public static string MakeValidFileName(this string text, char? replacement = '_', bool fancy = true)
    {
        StringBuilder sb = new StringBuilder(text.Length);
        var invalids = _invalids ?? (_invalids = Path.GetInvalidFileNameChars());
        bool changed = false;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (invalids.Contains(c))
            {
                changed = true;
                var repl = replacement ?? '\0';
                if (fancy)
                {
                    if (c == '"') repl = '”'; // U+201D right double quotation mark
                    else if (c == '\'') repl = '’'; // U+2019 right single quotation mark
                    else if (c == '/') repl = '⁄'; // U+2044 fraction slash
                }
                if (repl != '\0')
                    sb.Append(repl);
            }
            else
                sb.Append(c);
        }
        if (sb.Length == 0)
            return "_";
        return changed ? sb.ToString() : text;
    }


    public static string RemoveWhiteSpaces(this string str)
    {
        if (str.IsNullOrEmpty()) return "";
        return Regex.Replace(str, @"\s+", "");
    }


    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EmptyIfNull(this string value)
    {
        return (value ?? string.Empty).Trim();
    }
}