// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.MiscExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Utilities.ObjectPools;

namespace BuildingBlocks.Extensions;

public static class MiscExtensions
{
  public static void Dump(this Exception exception)
  {
    try
    {
      exception.StackTrace.Dump();
      exception.Message.Dump();
    }
    catch
    {
    }
  }

  public static string ToAllMessages(this Exception exception, bool includeStackTrace = false)
  {
    PooledStringBuilder pooledStringBuilder = PooledStringBuilder.Rent();
    StringBuilder sb = (StringBuilder) pooledStringBuilder;
    for (; exception != null; exception = exception.InnerException)
    {
      if (!sb.ToString().EmptyIfNull().Contains(exception.Message))
      {
        if (includeStackTrace)
        {
          if (sb.Length > 0)
          {
            sb.AppendLine();
            sb.AppendLine();
          }
          sb.AppendLine(exception.ToString());
        }
        else
          sb.Grow(exception.Message, " * ");
      }
    }
    return pooledStringBuilder.ToStringAndReturn();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToElapsedMinutes(this Stopwatch watch)
  {
    return "{0:0.0}".FormatWith((object) TimeSpan.FromMilliseconds((double) watch.ElapsedMilliseconds).TotalMinutes);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToElapsedSeconds(this Stopwatch watch)
  {
    return "{0:0.0}".FormatWith((object) TimeSpan.FromMilliseconds((double) watch.ElapsedMilliseconds).TotalSeconds);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsNullOrDefault<T>(this T? value) where T : struct
  {
    return default (T).Equals((object) value.GetValueOrDefault());
  }

  public static string ToHexString(this byte[] bytes, int length = 0)
  {
    if (bytes == null || bytes.Length == 0)
      return "";
    StringBuilder stringBuilder = new StringBuilder();
    foreach (byte num in bytes)
    {
      stringBuilder.Append(num.ToString("x2"));
      if (length > 0 && stringBuilder.Length >= length)
        break;
    }
    return stringBuilder.ToString();
  }

  public static void Grow(this StringBuilder sb, string grow, string delimiter)
  {
    Guard.AgainstNull((object) delimiter, nameof (delimiter));
    if (string.IsNullOrWhiteSpace(grow))
      return;
    if (sb.Length <= 0)
      sb.Append(grow);
    else
      sb.AppendFormat("{0}{1}", (object) delimiter, (object) grow);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string SafeGet(this string[] arr, int index)
  {
    return arr == null || index >= arr.Length ? "" : arr[index];
  }
}