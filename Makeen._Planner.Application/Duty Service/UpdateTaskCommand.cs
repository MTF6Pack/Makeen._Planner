using Domains;

namespace Makeen._Planner.Task_Service
{
    public class UpdateTaskCommand
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
    }
}
