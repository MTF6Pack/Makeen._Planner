using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Notification
    {
        public List<Task.Task>? Tasks { get; private set; }
        public string? Message { get; private set; }

        public Notification(List<Task.Task>? tasks, string? message)
        {
            Tasks = tasks;
            Message = message;
        }
        public Notification()
        {

        }
    }
}
