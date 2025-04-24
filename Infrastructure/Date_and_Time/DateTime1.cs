using System;
using System.Globalization;

namespace Infrastructure.Date_and_Time
{
    public readonly struct DateTime1
    {
        private readonly DateTime _dateTime;

        public DateTime1(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
        {
            _dateTime = new DateTime(year, month, day, hour, minute, second, new GregorianCalendar());
        }

        public DateTime1(DateTime dateTime)
        {
            // Always force Gregorian when assigning DateTime
            _dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, new GregorianCalendar());
        }

        public static implicit operator DateTime(DateTime1 dt) => dt._dateTime;
        public static implicit operator DateTime1(DateTime dt) => new(dt);

        public DateTime1 AddDays(int days) => new(_dateTime.AddDays(days));
        public DateTime1 AddMinutes(int minutes) => new(_dateTime.AddMinutes(minutes));
        public static DateTime1 Now => new(DateTime.UtcNow.AddMinutes(-210));
        public override string ToString()
        {
            return _dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public readonly DateTime ToDateTime() => _dateTime;
    }
}