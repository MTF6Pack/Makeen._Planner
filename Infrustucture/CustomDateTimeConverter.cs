using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrustucture
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(dateString))
                return default;

            // Try standard Gregorian formats.
            string[] gregorianFormats =
            [
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy/MM/dd HH:mm",
                "yyyy/MM/dd"
            ];

            if (DateTime.TryParseExact(dateString, gregorianFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                // If year is less than 1500, assume it was Persian.
                if (parsedDate.Year < 1500)
                    return DateHelper.ConvertPersianToGregorian(dateString);
                return parsedDate;
            }
            else
            {
                // Try converting as Persian.
                try
                {
                    return DateHelper.ConvertPersianToGregorian(dateString);
                }
                catch (Exception ex)
                {
                    throw new JsonException($"Unable to parse date: {dateString}", ex);
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // If time is midnight, write only the date.
            if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
                writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
            else
                writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm"));
        }
    }
}