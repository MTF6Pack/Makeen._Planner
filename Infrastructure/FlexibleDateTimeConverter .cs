using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(dateString))
                return default;

            // Try standard Gregorian formats
            //////        string[] gregorianFormats =
            //////        [
            //////    "yyyy-MM-dd HH:mm",
            //////    "yyyy-MM-dd",
            //////    "yyyy-MM-ddTHH:mm:ss",
            //////    "yyyy-MM-ddTHH:mm:ssZ",
            //////    "yyyy/MM/dd HH:mm",
            //////    "yyyy/MM/dd"
            //////];

            // Check if the date is Gregorian
            ////if (DateTime.TryParseExact(dateString, gregorianFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            ////{
            ////    if (parsedDate.Year < 1600)
            ////        return DateHelper.ConvertPersianToGregorian(parsedDate, true);
            ////    return parsedDate;
            ////}

            // If it's not Gregorian, assume it's Persian
            try
            {
                PersianCalendar persianCalendar = new();
                string[] persianFormats = ["yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy/MM/dd HH:mm", "yyyy/MM/dd"];

                if (DateTime.TryParseExact(dateString, persianFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime persianDate))
                {
                    int year = persianDate.Year;
                    int month = persianDate.Month;
                    int day = persianDate.Day;
                    int hour = persianDate.Hour;
                    int minute = persianDate.Minute;

                    // Convert Persian date to Gregorian
                    DateTime gregorianDate = persianCalendar.ToDateTime(year, month, day, hour, minute, 0, 0);
                    return gregorianDate;
                }
            }
            catch (Exception ex)
            {
                throw new JsonException($"Unable to parse Persian date: {dateString}", ex);
            }

            throw new JsonException($"Invalid date format: {dateString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // If the time is midnight (00:00:00), write only the date.
            if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
                writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
            else
                writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm"));
        }
    }
}