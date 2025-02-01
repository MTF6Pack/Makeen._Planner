using Domain.Task;

namespace Makeen._Planner.Task_Service
{
    public class AddTaskCommand
    {
        public Guid UserId { get; set; }
        public Guid? GroupId { get; set; }
        required public string Name { get; set; }
        public DateTime DeadLine { get; set; }
        public bool IsInGroup { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
        public Domain.Task.TaskStatus Status { get; set; }
    }
}
