using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrustucture
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] AcceptedFormats =
        [
        "yyyy-MM-dd-HH-mm",
        "yyyy/MM/dd/HH:mm",
        "yyyy/MM/dd-HH:mm",
        "yyyy-MM-dd HH-mm",
        "yyyy-MM-dd HH:mm",
        "yyyy/MM/dd HH:mm",
        "MM-dd-yyyy HH:mm",
        "dd-MM-yyyy HH:mm",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ss"
    ];

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString()!;

            if (DateTime.TryParseExact(dateString, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
            {
                return parsedDate;
            }

            throw new JsonException($"Invalid date format: {dateString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm"));
        }
    }
}

