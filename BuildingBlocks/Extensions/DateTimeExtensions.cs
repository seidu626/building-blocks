using System;
using System.Globalization;

namespace BuildingBlocks.Extensions
{
    public static class DateTimeExtensions
    {
        internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToEpochMilliseconds(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(Epoch).TotalMilliseconds;
        }

        public static long ToEpochSeconds(this DateTime dateTime)
        {
            return dateTime.ToEpochMilliseconds() / 1000L;
        }

        public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate, bool compareTime = false)
        {
            return compareTime ? date >= startDate && date <= endDate : date.Date >= startDate.Date && date.Date <= endDate.Date;
        }

        public static bool IsLastDayOfTheMonth(this DateTime dateTime)
        {
            return dateTime.AddDays(1).Day == 1;
        }

        public static bool IsWeekend(this DateTime value)
        {
            return value.DayOfWeek == DayOfWeek.Saturday || value.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsLeapYear(this DateTime value)
        {
            return DateTime.IsLeapYear(value.Year);
        }

        public static int Age(this DateTime birthDay)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDay.Year;
            if (birthDay > today.AddYears(-age)) age--;
            return age;
        }

        public static DateTime? ToUniversalTime(this DateTime? value)
        {
            return value?.ToUniversalTime();
        }

        public static DateTime? ToLocalTime(this DateTime? value)
        {
            return value?.ToLocalTime();
        }

        public static DateTime GetEvenHourDate(this DateTime? value)
        {
            var date = value ?? DateTime.UtcNow;
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0).AddHours(1);
        }

        public static DateTime GetEvenMinuteDate(this DateTime? value)
        {
            var date = value ?? DateTime.UtcNow;
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0).AddMinutes(1);
        }

        public static DateTime GetEvenMinuteDateBefore(this DateTime? value)
        {
            var date = value ?? DateTime.UtcNow;
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        public static long ToJavaScriptTicks(this DateTime value)
        {
            return (value.ToUniversalTime().Ticks - Epoch.Ticks) / 10000L;
        }

        public static DateTime GetFirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime ToEndOfTheDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        public static DateTime? ToEndOfTheDay(this DateTime? value)
        {
            return value?.ToEndOfTheDay();
        }

        public static long ToUnixTime(this DateTime value)
        {
            return Convert.ToInt64((value.ToUniversalTime() - Epoch).TotalSeconds);
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static string ToNativeString(this DateTime value, string format = null, IFormatProvider provider = null)
        {
            provider ??= CultureInfo.CurrentCulture;
            return value.ToString(format, provider).ReplaceNativeDigits(provider);
        }

        public static string ToIso8601String(this DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public static string ToShamsiDateYmd(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            return $"{persianCalendar.GetYear(date)}/{persianCalendar.GetMonth(date)}/{persianCalendar.GetDayOfMonth(date)}";
        }

        public static string ToShamsiDateDmy(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            return $"{persianCalendar.GetDayOfMonth(date)}/{persianCalendar.GetMonth(date)}/{persianCalendar.GetYear(date)}";
        }

        public static string ToShamsiDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);
            DayOfWeek dayOfWeek = persianCalendar.GetDayOfWeek(date);

            string monthName = month switch
            {
                1 => "فروردین",
                2 => "اردیبهشت",
                3 => "خرداد",
                4 => "تیر",
                5 => "مرداد",
                6 => "شهریور",
                7 => "مهر",
                8 => "آبان",
                9 => "آذر",
                10 => "دی",
                11 => "بهمن",
                12 => "اسفند",
                _ => string.Empty
            };

            string dayName = dayOfWeek switch
            {
                DayOfWeek.Saturday => "شنبه",
                DayOfWeek.Sunday => "یکشنبه",
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Tuesday => "سه شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Thursday => "پنجشنبه",
                DayOfWeek.Friday => "جمعه",
                _ => string.Empty
            };

            return $"{dayName} {day} {monthName} {year}";
        }
    }
}
