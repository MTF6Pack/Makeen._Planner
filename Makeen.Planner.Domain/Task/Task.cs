using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Domain.Task
{
    public class Task
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? GroupId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime DeadLine { get; private set; }
        public DateTime CreationTime { get; private set; }
        public bool IsInGroup { get; private set; }
        public TaskCategory TaskCategory { get; private set; }
        public PriorityCategory PriorityCategory { get; private set; }
        public TaskStatus Status { get; private set; }



        public Task(Guid userId, Guid? groupId, string name, DateTime deadLine, bool isInGroup,
            TaskCategory taskCategory, PriorityCategory priorityCategory, TaskStatus status)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            UserId = userId;
            GroupId = groupId;
            Name = name;
            DeadLine = deadLine;
            IsInGroup = isInGroup;
            TaskCategory = taskCategory;
            PriorityCategory = priorityCategory;
            Status = status;
        }
        public void UpdateTask(string name, DateTime deadline, TaskCategory taskCategory, PriorityCategory priorityCategory)
        {
            if (name != null) Name = name;
            DeadLine = deadline;
            TaskCategory = taskCategory;
            PriorityCategory = priorityCategory;
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
