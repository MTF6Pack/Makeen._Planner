using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Report
{
    public class Chart
    {
        public Chart(WeekDays weekDay, string doneOutofWhole, string pendingOutofWhole, int doneCount, int pendingCount)
        {
            Id = Guid.NewGuid();
            WeekDay = weekDay;
            DoneOutofWhole = doneOutofWhole;
            PendingOutofWhole = pendingOutofWhole;
            DoneCount = doneCount;
            PendingCount = pendingCount;
        }

        public Guid Id { get; private set; }
        public WeekDays WeekDay { get; private set; }
        public string DoneOutofWhole { get; private set; } = string.Empty;
        public string PendingOutofWhole { get; private set; } = string.Empty;
        public int DoneCount { get; private set; }
        public int PendingCount { get; private set; }

        public Chart()
        {

        }
    }
}
