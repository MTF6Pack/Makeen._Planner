using Domain.TaskEnums;
using System.Text.Json.Serialization;
using Status = Domain.TaskEnums.Status;

namespace Domain
{
    public class Task
    {
        public Guid? SenderId { get; private set; }
        public Guid Id { get; private set; }
        [JsonIgnore]
        public User? User { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Guid? GroupId { get; private set; }
        public bool? IsInGroup { get; private set; }
        public Status Status { get; private set; }
        public DateTime DeadLine { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime CreationTime { get; private set; }
        public PriorityCategory? PriorityCategory { get; private set; }
        public Alarm? Alarm { get; private set; }
        public Repeat? Repeat { get; private set; }
        public string? Description { get; private set; }
        public string? Result { get; private set; }
        public From From { get; private set; }
        public Task(Guid? groupId, string name, DateTime deadLine,
               PriorityCategory? priorityCategory, DateTime starttime, Repeat? repeat, Alarm? alarm, string? description, Guid? senderId)
        {
            Name = name;
            GroupId = groupId;
            Id = Guid.NewGuid();
            DeadLine = deadLine;
            StartTime = starttime;
            Status = (Status)2;
            CreationTime = DateTime.Now;
            SenderId = senderId;
            IsInGroup = SenderId.HasValue;
            PriorityCategory = priorityCategory == null ? TaskEnums.PriorityCategory.none : priorityCategory;
            Alarm = alarm == null ? TaskEnums.Alarm.None : alarm;
            Repeat = repeat == null ? TaskEnums.Repeat.None : repeat;
            Description = description;
            if (SenderId.HasValue && groupId is not null) From = From.Admin;
            else if (SenderId.HasValue && groupId is null) From = From.Friend;
            else From = From.MySelf;
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
                PriorityCategory = priorityCategory.Value;

            if (repeat.HasValue)
                Repeat = repeat.Value;

            if (alarm.HasValue)
                Alarm = alarm.Value;

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;
        }

        public void Done()
        {
            Status = (Status)1;
        }

        public void UpdateStatus(Status status)
        {
            Status = status;
        }
        public Task()
        {
        }
    }
}
