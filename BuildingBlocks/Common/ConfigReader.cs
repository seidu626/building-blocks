// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ConfigReader
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using BuildingBlocks.Common.Interfaces;
using BuildingBlocks.Extensions;

namespace BuildingBlocks.Common;

public sealed class ConfigReader : IConfigReader
{
  public ConfigReader()
  {
    FileInfo fileInfo = new FileInfo(new Uri(Assembly.GetCallingAssembly().Location).LocalPath + ".config");
    Ensure.Exists(fileInfo);
    this.Init(fileInfo, (XName) "add", "key", "value");
  }

  public ConfigReader(
    FileInfo configFile,
    XName element,
    string keyAttribute = "key",
    string valueAttribute = "value")
  {
    this.Init(configFile, element, keyAttribute, valueAttribute);
  }

  private void Init(
    FileInfo configFile,
    XName element,
    string keyAttribute,
    string valueAttribute)
  {
    Ensure.NotNull<FileInfo>(configFile, nameof (configFile));
    Ensure.NotNull<XName>(element, nameof (element));
    Ensure.NotNullOrEmptyOrWhiteSpace(keyAttribute);
    Ensure.NotNullOrEmptyOrWhiteSpace(valueAttribute);
    this.ConfigFile = Ensure.Exists(configFile);
    this.Settings = new Dictionary<string, string>();
    using (FileStream fileStream = this.ConfigFile.OpenRead())
    {
      foreach (XElement element1 in ((Stream) fileStream).GetElements(element))
      {
        XAttribute xattribute1 = element1.Attribute((XName) keyAttribute);
        XAttribute xattribute2 = element1.Attribute((XName) valueAttribute);
        if (xattribute1 != null && xattribute2 != null)
        {
          string key = xattribute1.Value;
          if (!key.IsNullOrEmptyOrWhiteSpace())
            this.Settings[key] = xattribute2.Value;
        }
      }
    }
  }

  public FileInfo ConfigFile { get; private set; }

  public Dictionary<string, string> Settings { get; private set; }

  public bool TryRead(string key, out IDictionary<string, string> values)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(key);
    using (FileStream fileStream = this.ConfigFile.OpenRead())
    {
      Dictionary<string, string> source = new Dictionary<string, string>();
      XElement[] array = ((Stream) fileStream).GetElements((XName) key).ToArray<XElement>();
      if (!((IEnumerable<XElement>) array).Any<XElement>())
      {
        values = (IDictionary<string, string>) null;
        return false;
      }
      if (array.Length > 1)
        throw new InvalidDataException("Multiple keys with the name: " + key + " was found.");
      foreach (XAttribute attribute in array[0].Attributes())
      {
        string key1 = attribute.Name.ToString();
        string str = attribute.Value;
        source[key1] = str;
      }
      if (source.Any<KeyValuePair<string, string>>())
      {
        values = (IDictionary<string, string>) source;
        return true;
      }
      values = (IDictionary<string, string>) null;
      return false;
    }
  }

  public bool TryRead(string key, out string value)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(key);
    return this.Settings.TryGetValue(key, out value);
  }

  public bool TryRead(string key, out short value)
  {
    string s;
    return this.TryGetString<short>(key, out value, out s) && short.TryParse(s, out value);
  }

  public bool TryRead(string key, out int value)
  {
    string s;
    return this.TryGetString<int>(key, out value, out s) && int.TryParse(s, out value);
  }

  public bool TryRead(string key, out long value)
  {
    string s;
    return this.TryGetString<long>(key, out value, out s) && long.TryParse(s, out value);
  }

  public bool TryRead(string key, out ushort value)
  {
    string s;
    return this.TryGetString<ushort>(key, out value, out s) && ushort.TryParse(s, out value);
  }

  public bool TryRead(string key, out uint value)
  {
    string s;
    return this.TryGetString<uint>(key, out value, out s) && uint.TryParse(s, out value);
  }

  public bool TryRead(string key, out ulong value)
  {
    string s;
    return this.TryGetString<ulong>(key, out value, out s) && ulong.TryParse(s, out value);
  }

  public bool TryRead(string key, out float value)
  {
    string s;
    return this.TryGetString<float>(key, out value, out s) && float.TryParse(s, out value);
  }

  public bool TryRead(string key, out double value)
  {
    string s;
    return this.TryGetString<double>(key, out value, out s) && double.TryParse(s, out value);
  }

  public bool TryRead(string key, out Decimal value)
  {
    string s;
    return this.TryGetString<Decimal>(key, out value, out s) && Decimal.TryParse(s, out value);
  }

  public bool TryRead(string key, out bool value)
  {
    string str;
    return this.TryGetString<bool>(key, out value, out str) && str.TryParseAsBool(out value);
  }

  public bool TryReadStringAsCSV(string key, string separator, out string[] value)
  {
    string str;
    if (!this.TryGetString<string[]>(key, out value, out str))
      return false;
    value = str.Split(new string[1]{ separator }, StringSplitOptions.RemoveEmptyEntries);
    return true;
  }

  public bool TryGetTicks(string key, out TimeSpan value)
  {
    long num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromTicks(num);
    return true;
  }

  public bool TryGetMilliseconds(string key, out TimeSpan value)
  {
    double num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromMilliseconds(num);
    return true;
  }

  public bool TryGetSeconds(string key, out TimeSpan value)
  {
    double num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromSeconds(num);
    return true;
  }

  public bool TryGetMinutes(string key, out TimeSpan value)
  {
    double num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromMinutes(num);
    return true;
  }

  public bool TryGetHours(string key, out TimeSpan value)
  {
    double num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromHours(num);
    return true;
  }

  public bool TryGetDays(string key, out TimeSpan value)
  {
    double num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromDays(num);
    return true;
  }

  public bool TryGetWeeks(string key, out TimeSpan value)
  {
    double num;
    if (!this.TryGetString<TimeSpan>(key, out value, out string _) || !this.TryRead(key, out num))
      return false;
    value = TimeSpan.FromDays(num * 7.0);
    return true;
  }

  public bool TryRead(string key, string formatSpecifier, out DateTime value)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(formatSpecifier);
    string s;
    return this.TryGetString<DateTime>(key, out value, out s) && DateTime.TryParseExact(s, formatSpecifier, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
  }

  public bool TryRead(string key, out FileInfo value)
  {
    string str;
    if (!this.TryRead(key, out str))
    {
      value = (FileInfo) null;
      return false;
    }
    value = new FileInfo(Path.GetFullPath(str));
    return true;
  }

  public bool TryRead(string key, out DirectoryInfo value)
  {
    string str;
    if (!this.TryRead(key, out str))
    {
      value = (DirectoryInfo) null;
      return false;
    }
    value = new DirectoryInfo(Path.GetFullPath(str));
    return true;
  }

  public bool TryRead(string key, out Uri value)
  {
    string uriString;
    if (!this.TryRead(key, out uriString))
    {
      value = (Uri) null;
      return false;
    }
    try
    {
      value = new Uri(uriString);
      return true;
    }
    catch (UriFormatException ex)
    {
      value = (Uri) null;
      return false;
    }
  }

  private bool TryGetString<T>(string key, out T defaultVal, out string value)
  {
    defaultVal = default (T);
    string str;
    if (!this.TryRead(key, out str))
    {
      value = (string) null;
      return false;
    }
    value = str;
    return true;
  }
}