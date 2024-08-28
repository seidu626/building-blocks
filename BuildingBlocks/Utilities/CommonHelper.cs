using System.Collections;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BuildingBlocks.ComponentModel;
using BuildingBlocks.ComponentModel.TypeConversion;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ReferenceEqualityComparer = System.Collections.Generic.ReferenceEqualityComparer;

namespace BuildingBlocks.Utilities;

/// <summary>
/// Represents a common helper
/// </summary>
public static partial class CommonHelper
{
    #region Fields

    //we use EmailValidator from FluentValidation. So let's keep them sync - https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Validators/EmailValidator.cs
    private const string EMAIL_EXPRESSION = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";

    private static readonly Regex _emailRegex;
    private static readonly IWebHostEnvironment HostingEnvironment;
    private static readonly IConfiguration Configuration;

    #endregion

    #region Ctor

    static CommonHelper()
    {
        HostingEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
        Configuration = EngineContext.Current.Resolve<IConfiguration>();
        _emailRegex = new Regex(EMAIL_EXPRESSION, RegexOptions.IgnoreCase);
    }
        
    private static bool? _isDevEnvironment;

    [ThreadStatic]
    private static Random _random;

    private static Random GetRandomizer()
    {
        if (_random == null)
        {
            _random = new Random();
        }

        return _random;
    }

        

    /// <summary>
    /// Maps a virtual path to a physical disk path.
    /// </summary>
    /// <param name="path">The path to map. E.g. "~/bin"</param>
    /// <param name="findAppRoot">Specifies if the app root should be resolved when mapped directory does not exist</param>
    /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
    /// <remarks>
    /// This method is able to resolve the web application root
    /// even when it's called during design-time (e.g. from EF design-time tools).
    /// </remarks>
    public static string MapPath(string path, bool findAppRoot = true)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        if (HostingEnvironment.WebRootPath != string.Empty)
        {
            // hosted
            return Path.Combine(HostingEnvironment.WebRootPath, path);
        }
        else
        {
            // not hosted. For example, running in unit tests or EF tooling
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');

            var testPath = Path.Combine(baseDirectory, path);

            if (findAppRoot /* && !Directory.Exists(testPath)*/)
            {
                // most likely we're in unit tests or design-mode (EF migration scaffolding)...
                // find solution root directory first
                var dir = FindSolutionRoot(baseDirectory);

                // concat the web root
                if (dir != null)
                {
                    baseDirectory = Path.Combine(dir.FullName, "Presentation\\SmartStore.Web");
                    testPath = Path.Combine(baseDirectory, path);
                }
            }

            return testPath;
        }
    }

    public static bool IsDevEnvironment
    {
        get
        {
            if (!_isDevEnvironment.HasValue)
            {
                _isDevEnvironment = IsDevEnvironmentInternal();
            }

            return _isDevEnvironment.Value;
        }
    }

    private static bool IsDevEnvironmentInternal()
    {
        if (HostingEnvironment.WebRootPath.IsNullOrEmpty())
            return true;

        if (HostingEnvironment.IsDevelopment())
            return true;

        if (System.Diagnostics.Debugger.IsAttached)
            return true;

        // if there's a 'SmartStore.NET.sln' in one of the parent folders,
        // then we're likely in a dev environment
        if (FindSolutionRoot(HostingEnvironment.WebRootPath) != null)
            return true;

        return false;
    }

    private static DirectoryInfo FindSolutionRoot(string currentDir)
    {
        var dir = Directory.GetParent(currentDir);
        while (true)
        {
            if (dir == null || IsSolutionRoot(dir))
                break;

            dir = dir.Parent;
        }

        return dir;
    }

    private static bool IsSolutionRoot(DirectoryInfo dir)
    {
        return File.Exists(Path.Combine(dir.FullName, "SmartStoreNET.sln"));
    }

    public static bool TryConvert<T>(object value, out T convertedValue)
    {
        convertedValue = default(T);

        if (TryConvert(value, typeof(T), CultureInfo.InvariantCulture, out object result))
        {
            if (result != null)
                convertedValue = (T)result;
            return true;
        }

        return false;
    }

    public static bool TryConvert<T>(object value, CultureInfo culture, out T convertedValue)
    {
        convertedValue = default(T);

        if (TryConvert(value, typeof(T), culture, out object result))
        {
            if (result != null)
                convertedValue = (T)result;
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryConvert(object value, Type to, out object convertedValue)
    {
        return TryConvert(value, to, CultureInfo.InvariantCulture, out convertedValue);
    }

    public static bool TryConvert(object value, Type to, CultureInfo culture, out object convertedValue)
    {
        if (to == null)
            throw new ArgumentNullException(nameof(to));

        convertedValue = null;

        if (value == null || value == DBNull.Value)
        {
            return to == typeof(string) || to.IsPredefinedSimpleType() == false;
        }

        if (to != typeof(object) && to.IsInstanceOfType(value))
        {
            convertedValue = value;
            return true;
        }

        Type from = value.GetType();

        if (culture == null)
        {
            culture = CultureInfo.InvariantCulture;
        }

        try
        {
            // Get a converter for 'to' (value -> to)
            var converter = TypeConverterFactory.GetConverter(to);
            if (converter != null && converter.CanConvertFrom(from))
            {
                convertedValue = converter.ConvertFrom(culture, value);
                return true;
            }

            // Try the other way round with a 'from' converter (to <- from)
            converter = TypeConverterFactory.GetConverter(from);
            if (converter != null && converter.CanConvertTo(to))
            {
                convertedValue = converter.ConvertTo(culture, null, value, to);
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    public static ExpandoObject ToExpando(object value)
    {
        Guard.AgainstNull(value, nameof(value));

        var anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(value);
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var item in anonymousDictionary)
        {
            expando.Add(item);
        }
        return (ExpandoObject)expando;
    }

    public static IDictionary<string, object> ObjectToDictionary(object obj)
    {
        if (obj == null)
            return new Dictionary<string, object>();

        return FastProperty.ObjectToDictionary(
            obj,
            key => key.Replace("_", "-").Replace("@", ""));
    }

    /// <summary>
    /// Gets a setting from the application's <c>web.config</c> <c>appSettings</c> node
    /// </summary>
    /// <typeparam name="T">The type to convert the setting value to</typeparam>
    /// <param name="key">The key of the setting</param>
    /// <param name="defValue">The default value to return if the setting does not exist</param>
    /// <returns>The casted setting value</returns>
    public static T GetAppSetting<T>(string key, T defValue = default(T))
    {
        Guard.AgainstNullOrEmpty(key, nameof(key));
        var setting = Configuration.GetValue<T>(key);
        return setting == null ? defValue : setting.Convert<T>();
    }

    public static bool HasConnectionString(string connectionStringName)
    {
        var conString = Configuration.GetConnectionString(connectionStringName);
        return conString != null && conString.HasValue();
    }

    private static bool TryAction<T>(Func<T> func, out T output)
    {
        Guard.AgainstNull(func, nameof(func));

        try
        {
            output = func();
            return true;
        }
        catch
        {
            output = default(T);
            return false;
        }
    }

    public static bool IsTruthy(object value)
    {
        if (value == null)
            return false;

        switch (value)
        {
            case string x:
                return x.HasValue();
            case bool x:
                return x == true;
            case DateTime x:
                return x > DateTime.MinValue;
            case TimeSpan x:
                return x > TimeSpan.MinValue;
            case Guid x:
                return x != Guid.Empty;
            case IComparable x:
                return x.CompareTo(0) != 0;
            case IEnumerable<object> x:
                return x.Any();
            case IEnumerable x:
                return x.GetEnumerator().MoveNext();
        }

        if (value.GetType().IsNullable(out var wrappedType))
        {
            return IsTruthy(Convert.ChangeType(value, wrappedType));
        }

        return true;
    }

    public static long GetObjectSizeInBytes(object obj, HashSet<object> instanceLookup = null)
    {
        if (obj == null)
            return 0;

        var type = obj.GetType();
        var genericArguments = type.GetGenericArguments();

        long size = 0;

        if (obj is string str)
        {
            size = Encoding.Default.GetByteCount(str);
        }
        else if (obj is StringBuilder sb)
        {
            size = Encoding.Default.GetByteCount(sb.ToString());
        }
        else if (type.IsEnum)
        {
            size = System.Runtime.InteropServices.Marshal.SizeOf(Enum.GetUnderlyingType(type));
        }
        else if (type.IsPredefinedSimpleType() || type.IsPredefinedGenericType())
        {
            //size = System.Runtime.InteropServices.Marshal.SizeOf(Nullable.GetUnderlyingType(type) ?? type); // crashes often
            size = 8; // mean/average
        }
        else if (obj is Stream stream)
        {
            size = stream.Length;
        }
        else if (obj is IDictionary dic)
        {
            foreach (var item in dic.Values)
            {
                size += GetObjectSizeInBytes(item, instanceLookup);
            }
        }
        else if (obj is IEnumerable e)
        {
            foreach (var item in e)
            {
                size += GetObjectSizeInBytes(item, instanceLookup);
            }
        }
        else
        {
            if (instanceLookup == null)
            {
                instanceLookup = new HashSet<object>(ReferenceEqualityComparer.Instance);
            }

            if (!type.IsValueType && instanceLookup.Contains(obj))
            {
                return 0;
            }

            instanceLookup.Add(obj);

            var serialized = false;

            if (type.IsSerializable && genericArguments.All(x => x.IsSerializable))
            {
                try
                {
                    using (var s = new MemoryStream())
                    {
                            
                        obj = Binary.Serialize(s.ToArray());
                        size = s.Length;

                        serialized = true;
                    }
                }
                catch { }
            }

            if (!serialized)
            {
                // Serialization failed or is not supported: make JSON.
                var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    ContractResolver = SmartContractResolver.Instance,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    MaxDepth = 10,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                size = Encoding.Default.GetByteCount(json);
            }
        }

        return size;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Ensures the subscriber email or throw.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <returns></returns>
    public static string EnsureSubscriberEmailOrThrow(string email)
    {
        var output = EnsureNotNull(email);
        output = output.Trim();
        output = EnsureMaximumLength(output, 255);

        if (!IsValidEmail(output))
        {
            throw new DomainException("Email is not valid.");
        }

        return output;
    }

    /// <summary>
    /// Verifies that a string is in valid e-mail format
    /// </summary>
    /// <param name="email">Email to verify</param>
    /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        email = email.Trim();

        return _emailRegex.IsMatch(email);
    }

    /// <summary>
    /// Verifies that string is an valid IP-Address
    /// </summary>
    /// <param name="ipAddress">IPAddress to verify</param>
    /// <returns>true if the string is a valid IpAddress and false if it's not</returns>
    public static bool IsValidIpAddress(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out var _);
    }

    /// <summary>
    /// Generate random digit code
    /// </summary>
    /// <param name="length">Length</param>
    /// <returns>Result string</returns>
    public static string GenerateRandomDigitCode(int length)
    {
        using var random = new SecureRandomNumberGenerator();
        var str = string.Empty;
        for (var i = 0; i < length; i++)
            str = string.Concat(str, random.Next(10).ToString());
        return str;
    }

    /// <summary>
    /// Returns an random integer number within a specified rage
    /// </summary>
    /// <param name="min">Minimum number</param>
    /// <param name="max">Maximum number</param>
    /// <returns>Result</returns>
    public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
    {
        using var random = new SecureRandomNumberGenerator();
        return random.Next(min, max);
    }

    /// <summary>
    /// Ensure that a string doesn't exceed maximum allowed length
    /// </summary>
    /// <param name="str">Input string</param>
    /// <param name="maxLength">Maximum length</param>
    /// <param name="postfix">A string to add to the end if the original string was shorten</param>
    /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
    public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if (str.Length <= maxLength) 
            return str;

        var pLen = postfix?.Length ?? 0;

        var result = str[0..(maxLength - pLen)];
        if (!string.IsNullOrEmpty(postfix))
        {
            result += postfix;
        }

        return result;
    }

    /// <summary>
    /// Ensures that a string only contains numeric values
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
    public static string EnsureNumericOnly(string str)
    {
        return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Ensure that a string is not null
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Result</returns>
    public static string EnsureNotNull(string str)
    {
        return str ?? string.Empty;
    }

    /// <summary>
    /// Indicates whether the specified strings are null or empty strings
    /// </summary>
    /// <param name="stringsToValidate">Array of strings to validate</param>
    /// <returns>Boolean</returns>
    public static bool AreNullOrEmpty(params string[] stringsToValidate)
    {
        return stringsToValidate.Any(string.IsNullOrEmpty);
    }

    /// <summary>
    /// Compare two arrays
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="a1">Array 1</param>
    /// <param name="a2">Array 2</param>
    /// <returns>Result</returns>
    public static bool ArraysEqual<T>(T[] a1, T[] a2)
    {
        //also see Enumerable.SequenceEqual(a1, a2);
        if (ReferenceEquals(a1, a2))
            return true;

        if (a1 == null || a2 == null)
            return false;

        if (a1.Length != a2.Length)
            return false;

        var comparer = EqualityComparer<T>.Default;
        return !a1.Where((t, i) => !comparer.Equals(t, a2[i])).Any();
    }
        
    /// <summary>
    /// Sets a property on an object to a value.
    /// </summary>
    /// <param name="instance">The object whose property to set.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to set the property to.</param>
    public static void SetProperty(object instance, string propertyName, object value)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        var instanceType = instance.GetType();
        var pi = instanceType.GetProperty(propertyName);
        if (pi == null)
            throw new DomainException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
        if (!pi.CanWrite)
            throw new DomainException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
        if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
            value = To(value, pi.PropertyType);
        pi.SetValue(instance, value, Array.Empty<object>());
    }

    /// <summary>
    /// Converts a value to a destination type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="destinationType">The type to convert the value to.</param>
    /// <returns>The converted value.</returns>
    public static object To(object value, Type destinationType)
    {
        return To(value, destinationType, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a value to a destination type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="destinationType">The type to convert the value to.</param>
    /// <param name="culture">Culture</param>
    /// <returns>The converted value.</returns>
    public static object To(object value, Type destinationType, CultureInfo culture)
    {
        if (value == null) 
            return null;

        var sourceType = value.GetType();

        var destinationConverter = TypeDescriptor.GetConverter(destinationType);
        if (destinationConverter.CanConvertFrom(value.GetType()))
            return destinationConverter.ConvertFrom(null, culture, value);

        var sourceConverter = TypeDescriptor.GetConverter(sourceType);
        if (sourceConverter.CanConvertTo(destinationType))
            return sourceConverter.ConvertTo(null, culture, value, destinationType);

        if (destinationType.IsEnum && value is int)
            return Enum.ToObject(destinationType, (int)value);

        if (!destinationType.IsInstanceOfType(value))
            return Convert.ChangeType(value, destinationType, culture);

        return value;
    }

    /// <summary>
    /// Converts a value to a destination type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <returns>The converted value.</returns>
    public static T To<T>(object value)
    {
        //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        return (T)To(value, typeof(T));
    }

    /// <summary>
    /// Convert enum for front-end
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Converted string</returns>
    public static string ConvertEnum(string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        var result = string.Empty;
        foreach (var c in str)
            if (c.ToString() != c.ToString().ToLower())
                result += " " + c.ToString();
            else
                result += c.ToString();

        //ensure no spaces (e.g. when the first letter is upper case)
        result = result.TrimStart();
        return result;
    }

    /// <summary>
    /// Get difference in years
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
    {
        //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
        //this assumes you are looking for the western idea of age and not using East Asian reckoning.
        var age = endDate.Year - startDate.Year;
        if (startDate > endDate.AddYears(-age))
            age--;
        return age;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the default file provider
    /// </summary>
    public static IFileProvider DefaultFileProvider { get; set; }

    #endregion
}