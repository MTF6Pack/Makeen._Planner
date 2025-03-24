//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Primitives;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Infrastructure
//{
//    public class GlobalDateTimeMiddleware(RequestDelegate next)
//    {
//        private readonly RequestDelegate _next = next;

//        public async Task InvokeAsync(HttpContext context)
//        {
//            if (context.Request.Query.Count == 0)
//            {
//                await _next(context);
//                return;
//            }

//            // Build new query parameters with converted date values.
//            var newQueryParameters = new Dictionary<string, StringValues>();
//            foreach (var kvp in context.Request.Query)
//            {
//                var convertedValues = kvp.Value.Select(ConvertValue!).ToArray();
//                newQueryParameters[kvp.Key] = new StringValues(convertedValues);
//            }

//            // Update the QueryString (this is the public property).
//            context.Request.QueryString = QueryString.Create(newQueryParameters);
//            Console.WriteLine($"Updated QueryString: {context.Request.QueryString}");

//            await _next(context);
//        }

//        private static string ConvertValue(string value)
//        {
//            if (string.IsNullOrWhiteSpace(value))
//                return value;

//            if (TryConvertToGregorian(value, out DateTime dt))
//            {
//                // Return only the date if time is exactly midnight.
//                return dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0
//                    ? dt.ToString("yyyy-MM-dd")
//                    : dt.ToString("yyyy-MM-dd HH:mm");
//            }

//            return value;
//        }

//        private static bool TryConvertToGregorian(string value, out DateTime dt)
//        {
//            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
//                return true;

//            try
//            {
//                dt = DateHelper.ConvertPersianToGregorian(value);
//                return true;
//            }
//            catch
//            {
//                dt = default;
//                return false;
//            }
//        }
//    }
//}