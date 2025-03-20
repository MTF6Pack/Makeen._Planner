using Domain;
using Domain.Task;
using Infrastructure;
using System.Text.Json.Serialization;

namespace Makeen._Planner.Task_Service
{
    public class UpdateTaskCommand
    {
        public string? Name { get; set; }
        public DateTime? DeadLine { get; set; }
        public DateTime? StartTime { get; set; }
        public PriorityCategory? PriorityCategory { get; set; }
        //public Domain.Task.TaskStatus Status { get; set; }
        public Alarm Alarm { get; set; }
        public Repeat Repeat { get; set; }
        public string? Description { get; set; }
    }
}
