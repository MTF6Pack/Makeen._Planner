using Domain.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chart
{
    public class AddChartCommand
    {
        public WeekDays WeekDay { get; private set; }
        public string DoneOutofWhole { get; private set; } = string.Empty;
        public string PendingOutofWhole { get; private set; } = string.Empty;
        public int DoneCount { get; private set; }
        public int PendingCount { get; private set; }
        public int AllTasksCount { get; private set; }
    }
}
