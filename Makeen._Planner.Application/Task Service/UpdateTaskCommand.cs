using Domain;
using Domain.Task;

namespace Makeen._Planner.Task_Service
{
    public class UpdateTaskCommand
    {
        required public string Name { get; set; }
        public DateTime DeadLine { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
        public Domain.Task.TaskStatus Status { get; set; }
    }
}
