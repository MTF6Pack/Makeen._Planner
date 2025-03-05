using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrustucture
{
    public static class HelperMethods
    {
        public static Guid ToGuid(this string text) => new(text);

        public static string GetPersianDayName(DayOfWeek dayOfWeek)
        {
            var persianDays = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Saturday, "شنبه" },
            { DayOfWeek.Sunday, "یکشنبه" },
            { DayOfWeek.Monday, "دوشنبه" },
            { DayOfWeek.Tuesday, "سه‌شنبه" },
            { DayOfWeek.Wednesday, "چهارشنبه" },
            { DayOfWeek.Thursday, "پنج‌شنبه" },
            { DayOfWeek.Friday, "جمعه" }
        };

            return persianDays[dayOfWeek];
        }
    }
}
