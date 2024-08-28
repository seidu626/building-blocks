// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.StreamExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class StreamExtensions
{
  private const char Cr = '\r';
  private const char Lf = '\n';
  private const char Null = '\0';

  [DebuggerStepThrough]
  public static long CountLines(this 
#nullable disable
    Stream stream, Encoding encoding = null)
  {
    Ensure.NotNull<Stream>(stream, nameof (stream));
    long num1 = 0;
    byte[] numArray = new byte[1048576];
    char ch = char.MinValue;
    char minValue = char.MinValue;
    if (encoding == null || object.Equals((object) encoding, (object) Encoding.ASCII) || object.Equals((object) encoding, (object) Encoding.UTF8))
    {
      int num2;
      while ((num2 = stream.Read(numArray, 0, numArray.Length)) > 0)
      {
        for (int index = 0; index < num2; ++index)
        {
          minValue = (char) numArray[index];
          if (ch != char.MinValue)
          {
            if ((int) minValue == (int) ch)
              ++num1;
          }
          else if (minValue == '\n' || minValue == '\r')
          {
            ch = minValue;
            ++num1;
          }
        }
      }
    }
    else
    {
      char[] chArray = new char[numArray.Length];
      int num3;
      while ((num3 = stream.Read(numArray, 0, numArray.Length)) > 0)
      {
        int chars = encoding.GetChars(numArray, 0, num3, chArray, 0);
        for (int index = 0; index < chars; ++index)
        {
          minValue = chArray[index];
          if (ch != char.MinValue)
          {
            if ((int) minValue == (int) ch)
              ++num1;
          }
          else if (minValue == '\n' || minValue == '\r')
          {
            ch = minValue;
            ++num1;
          }
        }
      }
    }
    if (minValue != '\n' && minValue != '\r' && minValue != char.MinValue)
      ++num1;
    return num1;
  }

  [DebuggerStepThrough]
  public static IEnumerable<XElement> GetElements(
    this Stream stream,
    XName name,
    bool ignoreCase = true)
  {
    Ensure.NotNull<Stream>(stream, nameof (stream));
    return stream.GetElements(name, new XmlReaderSettings(), ignoreCase);
  }

  [DebuggerStepThrough]
  public static IEnumerable<XElement> GetElements(
    this Stream stream,
    XName name,
    XmlReaderSettings settings,
    bool ignoreCase = true)
  {
    Ensure.NotNull<Stream>(stream, nameof (stream));
    Ensure.NotNull<XName>(name, nameof (name));
    Ensure.NotNull<XmlReaderSettings>(settings, nameof (settings));
    using (XmlReader reader = XmlReader.Create(stream, settings))
    {
      foreach (XElement eelement in reader.GetEelements(name, ignoreCase))
        yield return eelement;
    }
  }
}