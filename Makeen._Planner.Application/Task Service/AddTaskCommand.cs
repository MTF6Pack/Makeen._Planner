using Domain.Task;
using Infrustucture;
using System.Text.Json.Serialization;

namespace Makeen._Planner.Task_Service
{
    public class AddTaskCommand
    {
        public Guid? GroupId { get; set; }
        public Guid? ReceiverId { get; set; }
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime DeadLine { get; set; }
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime StartTime { get; set; }
        required public string Name { get; set; }
        public PriorityCategory? PriorityCategory { get; set; }
        public Alarm? Alarm { get; set; }
        public Repeat? Repeat { get; set; }
        public string? Description { get; set; }
    }
}
