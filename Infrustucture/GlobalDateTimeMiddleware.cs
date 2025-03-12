using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Infrustucture
{
    public class GlobalDateTimeMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var query = context.Request.Query;

            if (query.Count != 0)
            {
                // Build a new dictionary for query parameters.
                var newQueryParameters = new Dictionary<string, List<string>>();

                foreach (var kvp in query)
                {
                    var key = kvp.Key;
                    var values = kvp.Value;
                    var newValues = new List<string>();

                    foreach (var value in values)
                    {
                        bool isDate = false;
                        // First, try to parse with standard parsing.
                        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                        {
                            isDate = true;
                        }
                        else
                        {
                            // If standard parse fails, try our Persian conversion.
                            try
                            {
                                dt = DateHelper.ConvertPersianToGregorian(value!);
                                isDate = true;
                            }
                            catch
                            {
                                isDate = false;
                            }
                        }

                        if (isDate)
                        {
                            // If the parsed date has a year less than 1500, reconvert using our helper.
                            if (dt.Year < 1500)
                            {
                                try
                                {
                                    dt = DateHelper.ConvertPersianToGregorian(value!);
                                }
                                catch
                                {
                                    // If conversion fails, leave dt as parsed.
                                }
                            }
                            // Format the date to a standard Gregorian format.
                            // If time is midnight (i.e. no time was provided), output just the date.
                            if (dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0)
                                newValues.Add(dt.ToString("yyyy-MM-dd"));
                            else
                                newValues.Add(dt.ToString("yyyy-MM-dd HH:mm"));
                        }
                        else
                        {
                            // Not a date: preserve the original value.
                            newValues.Add(value!);
                        }
                    }

                    newQueryParameters[key] = newValues;
                }

                // Rebuild the query string from the updated parameters.
                var newQueryString = QueryString.Create(
                    newQueryParameters.SelectMany(kvp => kvp.Value.Select(val => new KeyValuePair<string, string>(kvp.Key, val)))
                );
                context.Request.QueryString = newQueryString;
            }

            // Continue processing the request.
            await _next(context);
        }
    }
}