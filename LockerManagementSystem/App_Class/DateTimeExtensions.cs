using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.App_Class
{
    public static class DateTimeExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static DateTime ToBatamTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        }

        public static int GetDayBetweenToday(this DateTime date)
        {
            var today = DateTime.UtcNow.ToBatamTime();
            var timediff = (date.Date - today.Date).TotalDays;

            return (int)timediff;
        }


        public static string OJTFormat(this DateTime? date)
        {
            if (date != null)
            {
                return ((DateTime)date).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static string OJTFormat(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string OJTFormat2(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy hh:mm tt");
        }

    }
}