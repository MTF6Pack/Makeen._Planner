using Domain;
using Domain.Task;
using Infrustucture;
using System.Text.Json.Serialization;

namespace Makeen._Planner.Task_Service
{
    public class UpdateTaskCommand
    {
        required public string Name { get; set; }
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime DeadLine { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
        public Domain.Task.TaskStatus Status { get; set; }
    }
}
