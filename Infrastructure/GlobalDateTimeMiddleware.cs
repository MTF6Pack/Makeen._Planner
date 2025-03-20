using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class GlobalDateTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalDateTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Handle query string date conversion globally
            await HandleQueryStringDateConversionAsync(context);

            // Process the request body for POST/PUT if necessary
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                await HandleRequestBodyDateConversionAsync(context);
            }

            // Continue to the next middleware
            await _next(context);
        }

        private async Task HandleQueryStringDateConversionAsync(HttpContext context)
        {
            var query = context.Request.Query;

            if (query.Count != 0)
            {
                var newQueryParameters = new Dictionary<string, List<string>>();

                foreach (var kvp in query)
                {
                    var key = kvp.Key;
                    var values = kvp.Value;
                    var newValues = new List<string>();

                    foreach (var value in values)
                    {
                        bool isDate = false;
                        DateTime dt;

                        // Try parsing as Persian date if applicable
                        if (DateHelper.IsPersianDate(value))
                        {
                            try
                            {
                                dt = DateHelper.ConvertPersianToGregorian(value);
                                isDate = true;
                            }
                            catch
                            {
                                isDate = false;
                            }
                        }
                        else
                        {
                            // Try parsing as Gregorian date
                            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            {
                                isDate = true;
                            }
                        }

                        if (isDate)
                        {
                            // Format the date to a standard Gregorian format
                            if (dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0)
                                newValues.Add(dt.ToString("yyyy-MM-dd"));
                            else
                                newValues.Add(dt.ToString("yyyy-MM-dd HH:mm"));
                        }
                        else
                        {
                            newValues.Add(value);
                        }
                    }

                    newQueryParameters[key] = newValues;
                }

                // Rebuild the query string from the updated parameters
                var newQueryString = QueryString.Create(
                    newQueryParameters.SelectMany(kvp => kvp.Value.Select(val => new KeyValuePair<string, string>(kvp.Key, val)))
                );
                context.Request.QueryString = newQueryString;
            }
        }

        private async Task HandleRequestBodyDateConversionAsync(HttpContext context)
        {
            var originalBodyStream = context.Request.Body;
            using (var memoryStream = new MemoryStream())
            {
                await context.Request.Body.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Assuming the request body is JSON, you can deserialize it and handle date conversion
                var bodyString = new StreamReader(memoryStream).ReadToEnd();

                // Modify the body string by converting Persian dates to Gregorian
                bodyString = ConvertDatesInRequestBody(bodyString);

                // Write the modified body back to the request body stream
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(bodyString));
            }
        }

        private string ConvertDatesInRequestBody(string bodyString)
        {
            // Add logic to convert Persian dates in the JSON body to Gregorian
            // Example: Regex or JSON deserialization could be used to replace Persian dates
            bodyString = bodyString.Replace("1404-01-01", "2025-03-20"); // Example placeholder logic
            return bodyString;
        }
    }
}