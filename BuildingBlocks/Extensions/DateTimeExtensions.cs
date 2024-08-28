// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.DateTimeExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Extensions;

public static class DateTimeExtensions
{
  internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

  public static long ToEpochMilliseconds(this DateTime dateTime)
  {
    return (long) dateTime.ToUniversalTime().Subtract(DateTimeExtensions.Epoch).TotalMilliseconds;
  }

  public static long ToEpochSeconds(this DateTime dateTime)
  {
    return dateTime.ToEpochMilliseconds() / 1000L;
  }

  public static bool IsBetween(
    this DateTime date,
    DateTime startDate,
    DateTime endDate,
    bool compareTime = false)
  {
    return !compareTime ? date.Date >= startDate.Date && date.Date <= endDate.Date : date >= startDate && date <= endDate;
  }

  public static bool IsLastDayOfTheMonth(this DateTime dateTime)
  {
    DateTime dateTime1 = dateTime;
    DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, 1);
    dateTime2 = dateTime2.AddMonths(1);
    DateTime dateTime3 = dateTime2.AddDays(-1.0);
    return dateTime1 == dateTime3;
  }

  public static bool IsWeekend(this DateTime value)
  {
    return value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday;
  }

  public static bool IsLeapYear(this DateTime value) => DateTime.DaysInMonth(value.Year, 2) == 29;

  public static int Age(this DateTime birthDay)
  {
    DateTime today = DateTime.Today;
    int num = today.Year - birthDay.Year;
    if (birthDay > today.AddYears(-num))
      --num;
    return num;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static DateTime? ToUniversalTime(this DateTime? value)
  {
    return !value.HasValue ? new DateTime?() : new DateTime?(value.Value.ToUniversalTime());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static DateTime? ToLocalTime(this DateTime? value)
  {
    return !value.HasValue ? new DateTime?() : new DateTime?(value.Value.ToLocalTime());
  }

  public static DateTime GetEvenHourDate(this DateTime? value)
  {
    if (!value.HasValue)
      value = new DateTime?(DateTime.UtcNow);
    DateTime dateTime = value.Value.AddHours(1.0);
    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
  }

  public static DateTime GetEvenMinuteDate(this DateTime? value)
  {
    if (!value.HasValue)
      value = new DateTime?(DateTime.UtcNow);
    DateTime dateTime = value.Value.AddMinutes(1.0);
    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
  }

  public static DateTime GetEvenMinuteDateBefore(this DateTime? value)
  {
    if (!value.HasValue)
      value = new DateTime?(DateTime.UtcNow);
    DateTime dateTime = value.Value;
    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
  }

  public static long ToJavaScriptTicks(this DateTime value)
  {
    return (((DateTimeOffset) value.ToUniversalTime()).Ticks - DateTimeExtensions.Epoch.Ticks) / 10000L;
  }

  public static DateTime GetFirstDayOfMonth(this DateTime value)
  {
    DateTime firstDayOfMonth = value;
    firstDayOfMonth = firstDayOfMonth.AddDays((double) -(firstDayOfMonth.Day - 1));
    return firstDayOfMonth;
  }

  public static DateTime GetLastDayOfMonth(this DateTime value)
  {
    DateTime lastDayOfMonth = value;
    lastDayOfMonth = lastDayOfMonth.AddMonths(1);
    lastDayOfMonth = lastDayOfMonth.AddDays((double) -lastDayOfMonth.Day);
    return lastDayOfMonth;
  }

  public static DateTime ToEndOfTheDay(this DateTime value)
  {
    return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
  }

  public static DateTime? ToEndOfTheDay(this DateTime? value)
  {
    return !value.HasValue ? value : new DateTime?(value.Value.ToEndOfTheDay());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static long ToUnixTime(this DateTime value)
  {
    return Convert.ToInt64((value.ToUniversalTime() - DateTimeExtensions.Epoch).TotalSeconds);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static DateTime FromUnixTime(this long unixTime)
  {
    return DateTimeExtensions.Epoch.AddSeconds((double) unixTime);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToNativeString(this DateTime value)
  {
    return value.ToNativeString((string) null, (IFormatProvider) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToNativeString(this DateTime value, IFormatProvider provider)
  {
    return value.ToNativeString((string) null, provider);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToNativeString(this DateTime value, string format)
  {
    return value.ToNativeString(format, (IFormatProvider) null);
  }

  public static string ToNativeString(
    this DateTime value,
    string format,
    IFormatProvider provider)
  {
    provider = (IFormatProvider) ((object) provider ?? (object) CultureInfo.CurrentCulture);
    return value.ToString(format, provider).ReplaceNativeDigits(provider);
  }

  public static string ToIso8601String(this DateTime value)
  {
    return value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
  }

  public static string ToShamsiDateYmd(this DateTime date)
  {
    PersianCalendar persianCalendar = new PersianCalendar();
    int year = ((Calendar) persianCalendar).GetYear(date);
    int month = ((Calendar) persianCalendar).GetMonth(date);
    int dayOfMonth = ((Calendar) persianCalendar).GetDayOfMonth(date);
    return year.ToString() + "/" + month.ToString() + "/" + dayOfMonth.ToString();
  }

  public static string ToShamsiDateDmy(this DateTime date)
  {
    PersianCalendar persianCalendar = new PersianCalendar();
    int year = ((Calendar) persianCalendar).GetYear(date);
    int month = ((Calendar) persianCalendar).GetMonth(date);
    return ((Calendar) persianCalendar).GetDayOfMonth(date).ToString() + "/" + month.ToString() + "/" + year.ToString();
  }

  public static string ToShamsiDate(this DateTime date)
  {
    PersianCalendar persianCalendar = new PersianCalendar();
    int year = ((Calendar) persianCalendar).GetYear(date);
    int month = ((Calendar) persianCalendar).GetMonth(date);
    int dayOfMonth = ((Calendar) persianCalendar).GetDayOfMonth(date);
    DayOfWeek dayOfWeek = ((Calendar) persianCalendar).GetDayOfWeek(date);
    string str1;
    switch (month)
    {
      case 1:
        str1 = "فروردین";
        break;
      case 2:
        str1 = "اردیبهشت";
        break;
      case 3:
        str1 = "خرداد";
        break;
      case 4:
        str1 = "تیر";
        break;
      case 5:
        str1 = "مرداد";
        break;
      case 6:
        str1 = "شهریور";
        break;
      case 7:
        str1 = "مهر";
        break;
      case 8:
        str1 = "آبان";
        break;
      case 9:
        str1 = "آذر";
        break;
      case 10:
        str1 = "دی";
        break;
      case 11:
        str1 = "بهمن";
        break;
      case 12:
        str1 = "اسفند";
        break;
      default:
        str1 = "";
        break;
    }
    string str2;
    switch (dayOfWeek)
    {
      case DayOfWeek.Sunday:
        str2 = "یکشنبه";
        break;
      case DayOfWeek.Monday:
        str2 = "دوشنبه";
        break;
      case DayOfWeek.Tuesday:
        str2 = "سه شنبه";
        break;
      case DayOfWeek.Wednesday:
        str2 = "چهارشنبه";
        break;
      case DayOfWeek.Thursday:
        str2 = "پنجشنبه";
        break;
      case DayOfWeek.Friday:
        str2 = "جمعه";
        break;
      case DayOfWeek.Saturday:
        str2 = "شنبه";
        break;
      default:
        str2 = "";
        break;
    }
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
    interpolatedStringHandler.AppendFormatted(str2);
    interpolatedStringHandler.AppendLiteral(" ");
    interpolatedStringHandler.AppendFormatted(dayOfMonth.ToString());
    interpolatedStringHandler.AppendLiteral(" ");
    interpolatedStringHandler.AppendFormatted(str1);
    interpolatedStringHandler.AppendLiteral(" ");
    interpolatedStringHandler.AppendFormatted(year.ToString());
    return interpolatedStringHandler.ToStringAndClear();
  }
}