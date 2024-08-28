using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Extensions;

public static class ObjectExtensions
{
    // Serialize an object to XML string
    public static string SerializeObject<T>(this T toSerialize)
    {
        using (var stringWriter = new StringWriter())
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringWriter, toSerialize);
            return stringWriter.ToString();
        }
    }

    // Convert object properties to a dictionary
    public static RouteValueDictionary ObjectToDictionary(this object value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(value));
    }

    // Cast KeyValuePair to object key-value pairs
    public static KeyValuePair<object, object> Cast<K, V>(this KeyValuePair<K, V> kvp)
    {
        return new KeyValuePair<object, object>(kvp.Key, kvp.Value);
    }

    // Deserialize JSON to object of type T
    public static T Map<T>(string jsonString) where T : class
    {
        return JsonSerializer.Deserialize<T>(jsonString);
    }

    // Convert an IDictionary to a dynamic object
    public static dynamic ToObject(this IDictionary<string, object> dict)
    {
        var expando = new ExpandoObject();
        var collection = (ICollection<KeyValuePair<string, object>>)expando;
        foreach (var pair in dict)
        {
            collection.Add(pair);
        }
        return expando;
    }

    // Generic method to convert IDictionary to a specified type
    public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
    {
        var obj = new T();
        foreach (var item in source)
        {
            PropertyInfo prop = obj.GetType().GetProperty(item.Key);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, Convert.ChangeType(item.Value, prop.PropertyType), null);
            }
        }
        return obj;
    }

    // Extract property value from object based on property name
    public static object GetPropertyValue(this object obj, string propertyName)
    {
        foreach (var name in propertyName.Split('.'))
        {
            if (obj == null) return null;
            var prop = obj.GetType().GetProperty(name);
            if (prop == null) return null;
            obj = prop.GetValue(obj, null);
        }
        return obj;
    }

    // Safely attempt to convert any type to another type
    public static bool TryConvert<T>(this object source, out T result)
    {
        try
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (source != null && converter.CanConvertFrom(source.GetType()))
            {
                result = (T)converter.ConvertFrom(source);
                return true;
            }
        }
        catch
        {
            // Log or handle the error if necessary
        }
        result = default;
        return false;
    }

    // Convert an object to dictionary with reflection
    public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        if (source == null) return null;
        var dict = new Dictionary<string, object>();
        PropertyInfo[] properties = source.GetType().GetProperties(bindingFlags);
        foreach (PropertyInfo property in properties)
        {
            if (property.CanRead)
            {
                dict.Add(property.Name, property.GetValue(source, null));
            }
        }
        return dict;
    }
}