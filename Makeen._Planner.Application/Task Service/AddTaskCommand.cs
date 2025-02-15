using Domain.Task;

namespace Makeen._Planner.Task_Service
{
    public class AddTaskCommand
    {
        public Guid UserId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime DeadLine { get; set; }
        required public string Name { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
    }
}
