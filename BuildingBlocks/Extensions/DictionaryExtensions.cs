// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.DictionaryExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using BuildingBlocks.Utilities;

namespace BuildingBlocks.Extensions;

public static class DictionaryExtensions
{
  public static string SerializeDictionaryToXml(this Dictionary<string, string> dictionary)
  {
    using (StringWriter stringWriter = new StringWriter())
    {
      new XmlSerializer(typeof (Dictionary<string, string>)).Serialize((TextWriter) stringWriter, (object) dictionary);
      return stringWriter.ToString();
    }
  }

  public static TValue GetValueOrDefault<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    TKey key)
  {
    TValue valueOrDefault;
    dictionary.TryGetValue(key, out valueOrDefault);
    return valueOrDefault;
  }

  public static void AddRange<TKey, TValue>(
    this IDictionary<TKey, TValue> values,
    IEnumerable<KeyValuePair<TKey, TValue>> other)
  {
    foreach (KeyValuePair<TKey, TValue> keyValuePair in other)
    {
      if (values.ContainsKey(keyValuePair.Key))
        throw new ArgumentException("An item with the same key has already been added.");
      values.Add(keyValuePair);
    }
  }

  public static IDictionary<string, object> Merge(
    this IDictionary<string, object> instance,
    string key,
    object value,
    bool replaceExisting = true)
  {
    if (replaceExisting || !instance.ContainsKey(key))
      instance[key] = value;
    return instance;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IDictionary<string, object> Merge(
    this IDictionary<string, object> instance,
    object values,
    bool replaceExisting = true)
  {
    return instance.Merge<string, object>(CommonHelper.ObjectToDictionary(values), replaceExisting);
  }

  public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
    this IDictionary<TKey, TValue> instance,
    IDictionary<TKey, TValue> from,
    bool replaceExisting = true)
  {
    foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) from)
    {
      if (replaceExisting || !instance.ContainsKey(keyValuePair.Key))
        instance[keyValuePair.Key] = keyValuePair.Value;
    }
    return instance;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IDictionary<string, object> AppendInValue(
    this IDictionary<string, object> instance,
    string key,
    string separator,
    string value)
  {
    return DictionaryExtensions.AddInValue(instance, key, separator, value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IDictionary<string, object> PrependInValue(
    this IDictionary<string, object> instance,
    string key,
    string separator,
    string value)
  {
    return DictionaryExtensions.AddInValue(instance, key, separator, value, true);
  }

  private static IDictionary<string, object> AddInValue(
    IDictionary<string, object> instance,
    string key,
    string separator,
    string value,
    bool prepend = false)
  {
    object obj;
    if (!instance.TryGetValue(key, out obj))
    {
      instance[key] = (object) value;
    }
    else
    {
      IEnumerable<string> strings1 = ((IEnumerable<string>) obj.ToString().Trim().Split(new string[1]
      {
        separator
      }, StringSplitOptions.RemoveEmptyEntries)).AsEnumerable<string>();
      IEnumerable<string> strings2 = ((IEnumerable<string>) value.Trim().Split(new string[1]
      {
        separator
      }, StringSplitOptions.RemoveEmptyEntries)).AsEnumerable<string>();
      IEnumerable<string> values = prepend ? strings2.Union<string>(strings1) : strings1.Union<string>(strings2);
      instance[key] = (object) string.Join(separator, values);
    }
    return instance;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key)
  {
    if (instance == null)
      throw new ArgumentNullException(nameof (instance));
    TValue obj;
    instance.TryGetValue(key, out obj);
    return obj;
  }

  public static ExpandoObject ToExpandoObject(
    this IDictionary<string, object> source,
    bool castIfPossible = false)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    if (castIfPossible && source is ExpandoObject)
      return source as ExpandoObject;
    ExpandoObject values = new ExpandoObject();
    ((IDictionary<string, object>) values).AddRange<string, object>((IEnumerable<KeyValuePair<string, object>>) source);
    return values;
  }

  public static bool TryAdd<TKey, TValue>(
    this IDictionary<TKey, TValue> source,
    TKey key,
    TValue value,
    bool updateIfExists = false)
  {
    if (source == null || (object) key == null)
      return false;
    if (source.ContainsKey(key))
    {
      if (!updateIfExists)
        return false;
      source[key] = value;
      return true;
    }
    source.Add(key, value);
    return true;
  }

  public static bool TryRemove<TKey, TValue>(
    this IDictionary<TKey, TValue> source,
    TKey key,
    out TValue value)
  {
    value = default (TValue);
    if (source == null || (object) key == null || !source.TryGetValue(key, out value))
      return false;
    source.Remove(key);
    return true;
  }
}