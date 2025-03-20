using System;
using System.Globalization;

namespace Infrastructure
{
    public static class DateHelper
    {
        private static readonly PersianCalendar persianCalendar = new();

        /// <summary>
        /// Converts a Persian (Jalali) date string to a Gregorian DateTime.
        /// </summary>
        public static DateTime ConvertPersianToGregorian(string persianDateString)
        {
            if (string.IsNullOrWhiteSpace(persianDateString))
                throw new ArgumentNullException(nameof(persianDateString));

            string[] persianFormats =
            {
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd",
                "yyyy-MM-dd"
            };

            CultureInfo persianCulture = new("fa-IR");

            // Check for valid Persian date parsing
            foreach (var format in persianFormats)
            {
                if (DateTime.TryParseExact(persianDateString, format, persianCulture, DateTimeStyles.None, out DateTime persianDate))
                {
                    DateTime gregorianDate = persianCalendar.ToDateTime(persianDate.Year, persianDate.Month, persianDate.Day, 0, 0, 0, 0);
                    bool hasTime = persianDateString.Contains(':');
                    if (hasTime)
                    {
                        return new DateTime(gregorianDate.Year, gregorianDate.Month, gregorianDate.Day, persianDate.Hour, persianDate.Minute, 0);
                    }
                    else
                    {
                        return gregorianDate.Date;
                    }
                }
            }

            return DateTime.Parse(persianDateString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks if a string is in Persian date format.
        /// </summary>
        public static bool IsPersianDate(string dateString)
        {
            string[] persianFormats =
            {
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd",
                "yyyy-MM-dd"
            };

            foreach (var format in persianFormats)
            {
                if (DateTime.TryParseExact(dateString, format, new CultureInfo("fa-IR"), DateTimeStyles.None, out _))
                {
                    return true;
                }
            }
            return false;
        }
    }
}