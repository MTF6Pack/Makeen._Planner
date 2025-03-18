using System;
using System.Globalization;

namespace Infrastructure
{
    public static class DateHelper
    {
        private static readonly PersianCalendar persianCalendar = new();

        /// <summary>
        /// Converts a Persian (Jalali) date string to a Gregorian DateTime.
        /// Ensures that seconds are always removed.
        /// </summary>
        public static DateTime ConvertPersianToGregorian(string persianDateString)
        {
            if (string.IsNullOrWhiteSpace(persianDateString)) throw new ArgumentNullException(nameof(persianDateString));

            string[] persianFormats =
            [
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd",
                "yyyy-MM-dd"
            ];

            CultureInfo persianCulture = new("fa-IR");

            foreach (var format in persianFormats)
            {
                if (DateTime.TryParseExact(persianDateString, format, persianCulture, DateTimeStyles.None, out DateTime persianDate))
                {
                    if (persianDate.Year < 1500)
                    {
                        DateTime gregorianDate = persianCalendar.ToDateTime(persianDate.Year, persianDate.Month, persianDate.Day, persianDate.Hour, persianDate.Minute, 0, 0);
                        bool hasTime = persianDateString.Contains(':');
                        return hasTime ? gregorianDate : gregorianDate.Date;
                    }
                    else
                    {
                        bool hasTime = persianDateString.Contains(':');
                        return hasTime ? persianDate : persianDate.Date;
                    }
                }
            }

            DateTime parsedDate = DateTime.Parse(persianDateString, CultureInfo.InvariantCulture);
            return new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, parsedDate.Hour, parsedDate.Minute, 0);
        }

        /// <summary>
        /// Converts a Gregorian DateTime to a Persian (Jalali) date string.
        /// </summary>
        /// <param name="gregorianDate">The Gregorian DateTime to convert.</param>
        /// <param name="includeTime">If true, includes the time in the output string.</param>
        /// <returns>A Persian date string in "yyyy/MM/dd" or "yyyy/MM/dd HH:mm" format.</returns>
        public static string ConvertGregorianToPersian(DateTime gregorianDate, bool includeTime = false)
        {
            int year = persianCalendar.GetYear(gregorianDate);
            int month = persianCalendar.GetMonth(gregorianDate);
            int day = persianCalendar.GetDayOfMonth(gregorianDate);
            string persianDate = $"{year:D4}/{month:D2}/{day:D2}";

            if (includeTime)
            {
                int hour = gregorianDate.Hour;
                int minute = gregorianDate.Minute;
                persianDate += $" {hour:D2}:{minute:D2}";
            }

            return persianDate;
        }
    }
}
