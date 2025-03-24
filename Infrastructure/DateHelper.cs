using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Infrastructure
{
    public static class DateHelper
    {
        private static readonly PersianCalendar persianCalendar = new();

        public static DateTime ConvertPersianToGregorian(DateTime persianDate, bool includeTime)
        {
            if (persianDate.Year > 1600) return persianDate;

            int year = persianCalendar.GetYear(persianDate);
            int month = persianCalendar.GetMonth(persianDate);
            int day = persianCalendar.GetDayOfMonth(persianDate);

            int hour = includeTime ? persianDate.Hour : 0;
            int minute = includeTime ? persianDate.Minute : 0;

            return persianCalendar.ToDateTime(year, month, day, hour, minute, 0, 0);
        }

        public static (DateTime persianDate, string formatted) ConvertGregorianToPersian(DateTime gregorianDate, bool includeTime = false)
        {
            if (gregorianDate.Year < 1600) return (gregorianDate, "");

            int year = persianCalendar.GetYear(gregorianDate);
            int month = persianCalendar.GetMonth(gregorianDate);
            int day = persianCalendar.GetDayOfMonth(gregorianDate);
            int hour = includeTime ? gregorianDate.Hour : 0;
            int minute = includeTime ? gregorianDate.Minute : 0;

            DateTime persianDate = new(year, month, day, hour, minute, 0, persianCalendar);

            // Use a format string that always displays the time if includeTime is true.
            string formatted = includeTime
                ? persianDate.ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture)
                : persianDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            return (persianDate, formatted);
        }

    }
}