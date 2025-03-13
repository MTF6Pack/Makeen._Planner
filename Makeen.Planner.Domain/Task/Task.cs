using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Domain.Task
{
    public class Task
    {
        public Guid? SenderId { get; private set; }
        //public Guid? Userid { get; set; }
        public Guid Id { get; private set; }
        [JsonIgnore]
        public User? User { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Guid? GroupId { get; private set; }
        public bool? IsInGroup { get; private set; }
        public TaskStatus Status { get; private set; }
        public DateTime DeadLine { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime CreationTime { get; private set; }
        public PriorityCategory? PriorityCategory { get; private set; }
        public Alarm Alarm { get; private set; }
        public Repeat? Repeat { get; private set; }
        public string? Description { get; private set; }
        public Task(Guid? groupId, string name, DateTime deadLine,
               PriorityCategory? priorityCategory, DateTime starttime, Repeat? repeat, Alarm alarm, string? description, Guid? senderId)
        {
            Name = name;
            GroupId = groupId;
            Id = Guid.NewGuid();
            DeadLine = deadLine;
            StartTime = starttime;
            Status = (TaskStatus)2;
            CreationTime = DateTime.Now;
            IsInGroup = SenderId.HasValue;
            PriorityCategory = priorityCategory;
            Alarm = alarm;
            Repeat = repeat;
            Description = description;
            SenderId = senderId;
        }


        public void UpdateTask(string? name, DateTime? deadLine, PriorityCategory? priorityCategory, DateTime? starttime, Repeat? repeat, Alarm? alarm, string? description)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if (deadLine.HasValue)
                DeadLine = deadLine.Value;

            if (starttime.HasValue)
                StartTime = starttime.Value;

            if (priorityCategory.HasValue)
                PriorityCategory = (PriorityCategory)priorityCategory.Value;

            if (repeat.HasValue)
                Repeat = (Repeat)repeat.Value;

            if (alarm.HasValue)
                Alarm = (Alarm)alarm.Value;

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;
        }

        public void Done()
        {
            Status = (TaskStatus)1;
        }

        public void UpdateTaskStatus(TaskStatus status)
        {
            Status = status;
        }
        public Task()
        {
        }
    }
}
