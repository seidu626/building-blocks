using System.Text.RegularExpressions;

namespace BuildingBlocks.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan ParseTimeSpan(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty", nameof(input));

        // Match the input string with the time pattern
        var regex = new Regex(@"^(\d+)([smhd])$", RegexOptions.IgnoreCase);
        var match = regex.Match(input);

        if (!match.Success)
            throw new ArgumentException($"Invalid time span format: {input}", nameof(input));

        var value = int.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value.ToLower();

        return unit switch
        {
            "s" => TimeSpan.FromSeconds(value),
            "m" => TimeSpan.FromMinutes(value),
            "h" => TimeSpan.FromHours(value),
            "d" => TimeSpan.FromDays(value),
            _ => throw new ArgumentException($"Invalid time unit: {unit}", nameof(input))
        };
    }
}
