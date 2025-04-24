using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Date_and_Time
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(dateString)) return default;

            string[] JalaliFormats = [
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd",
                "yyyy/MM/dd"];

            if (DateTime.TryParseExact(dateString, JalaliFormats, CultureInfo.DefaultThreadCurrentUICulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                if (parsedDate.Year > 1600) return DateHelper.ConvertGregorianToPersian(parsedDate, true).persianDate;
                return parsedDate;
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm"));
        }
    }
}