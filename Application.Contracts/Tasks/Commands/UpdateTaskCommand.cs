using Domain.TaskEnums;

namespace Application.Contracts.Tasks.Commands
{
    public class UpdateTaskCommand
    {
        public string? Name { get; set; }
        public DateTime? DeadLine { get; set; }
        public DateTime? StartTime { get; set; }
        public PriorityCategory? PriorityCategory { get; set; }
        //public Domain.Task.Status Status { get; set; }
        public Alarm Alarm { get; set; }
        public Repeat Repeat { get; set; }
        public string? Description { get; set; }
    }
}
