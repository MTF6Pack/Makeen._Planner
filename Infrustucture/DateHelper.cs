using System;
using System.Globalization;

namespace Infrustucture
{
    public static class DateHelper
    {
        private static readonly PersianCalendar persianCalendar = new();

        /// <summary>
        /// Converts a Persian (Jalali) date string to a Gregorian DateTime.
        /// Accepts dates with or without time. If time is missing, 00:00 is assumed.
        /// If the parsed date’s year is less than 1500, it is assumed to be Persian.
        /// </summary>
        /// <param name="persianDateString">A date string, for example "1403/12/22" or "1403/12/22 15:45".</param>
        /// <returns>The corresponding Gregorian DateTime.</returns>
        public static DateTime ConvertPersianToGregorian(string persianDateString)
        {
            if (string.IsNullOrWhiteSpace(persianDateString))
                throw new ArgumentNullException(nameof(persianDateString));

            // Define possible formats for Persian date strings (with and without time).
            string[] persianFormats =
            [
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd",
                "yyyy-MM-dd"
            ];

            // Use Persian culture ("fa-IR") for parsing.
            CultureInfo persianCulture = new("fa-IR");

            foreach (var format in persianFormats)
            {
                if (DateTime.TryParseExact(persianDateString, format, persianCulture, DateTimeStyles.None, out DateTime persianDate))
                {
                    // If the parsed year is less than 1500, assume it's a Persian date.
                    if (persianDate.Year < 1500)
                    {
                        int year = persianDate.Year;
                        int month = persianDate.Month;
                        int day = persianDate.Day;
                        int hour = persianDate.Hour;    // will be 0 if time is missing
                        int minute = persianDate.Minute; // will be 0 if time is missing
                        int second = persianDate.Second; // will be 0 if time is missing
                        return persianCalendar.ToDateTime(year, month, day, hour, minute, second, 0);
                    }
                    else
                    {
                        // Otherwise, it's already a Gregorian date.
                        return persianDate;
                    }
                }
            }

            // If none of the Persian formats matched, attempt a fallback parse (assuming Gregorian).
            return DateTime.Parse(persianDateString, CultureInfo.InvariantCulture);
        }
    }
}