// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.DatetimeProvider
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Globalization;

namespace BuildingBlocks.Helpers;

public class DatetimeProvider : IDatetimeProvider
{
  public DateTime DateTimeUtcNow => DateTime.UtcNow;

  public DateTime DateTimeNow => DateTime.Now;

  public string GetDateTimeNow(string format = "MM/dd/yyyy HH:mm:ss.fff zzz")
  {
    return DateTime.Now.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
  }

  public string GetDateTimeUtcNow(string format = "MM/dd/yyyy HH:mm:ss.fff zzz")
  {
    return DateTime.UtcNow.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
  }

  public static class Formats
  {
    public const string CatsPreferredFormat = "MM/dd/yyyy HH:mm:ss.fff zzz";
    public const string CatsAltFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";
    public const string CatRFC3339 = "yyyy-MM-dd HH:mm:ss.fffzzz";
    public const string RFC3339Long = "yyyy-MM-dd'T'HH:mm:ss.ffffffK";
    public const string RFC3339Short = "yyyy-MM-dd'T'HH:mm:ss.fffK";
    public const string RFC3339Shortest = "yyyy-MM-dd'T'HH:mm:ssK";
  }
}