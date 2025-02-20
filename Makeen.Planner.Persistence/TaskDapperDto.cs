using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Task_Service
{
    public class TaskDapperDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public int FutureTasksCount { get; set; }
    }
}
