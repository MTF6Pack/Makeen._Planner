using Domain.Task;
using Infrustucture;
using System.Text.Json.Serialization;

namespace Makeen._Planner.Task_Service
{
    public class AddTaskCommand
    {
        public Guid? GroupId { get; set; }
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime DeadLine { get; set; }
        required public string Name { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
    }
}
