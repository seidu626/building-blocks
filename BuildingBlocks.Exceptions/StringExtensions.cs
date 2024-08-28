using System.Globalization;

namespace BuildingBlocks.Exceptions;

internal static class StringExtensions
{
    public static string FormatCurrent(this string format, params object[] objects)
    {
        return string.Format(CultureInfo.CurrentCulture, format, objects);
    }
    
    public static string FormatInvariant(this string format, params object[] objects)
    {
        return string.Format(CultureInfo.InvariantCulture, format, objects);
    }

}