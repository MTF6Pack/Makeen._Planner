using Domain.TaskEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helpers
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

        public static string TranslateAlarm(Alarm alarm) => alarm switch
        {
            Alarm.Fifteen_Minutes => "یک ربع",
            Alarm.Thirty_Minutes => "نیم ساعت",
            Alarm.One_Hour => "یک ساعت",
            Alarm.One_Day => "یک روز",
            _ => "نامشخص"
        };
    }
}
